using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Baker : MonoBehaviour
{
    private Animator _animator;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        StartCoroutine(Bake());
    }

    private IEnumerator Bake()
    {
        Mesh mesh = new Mesh();
        int vertsCount = Mathf.NextPowerOfTwo(_skinnedMeshRenderer.sharedMesh.vertexCount);
        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            int framesCount = Mathf.NextPowerOfTwo(Mathf.RoundToInt(clip.length * 60));
            Texture2D positions = new Texture2D(vertsCount, framesCount, TextureFormat.RGBAFloat, false);
            positions.name = clip.name;
            for (int i = 0; i < framesCount; i++)
            {
                float fraction = (float) i / framesCount; 
                _animator.SetFloat("Progress", fraction);
                yield return null;
                _skinnedMeshRenderer.BakeMesh(mesh);
                for (var j = 0; j < mesh.vertexCount; j++)
                {
                    var vert = mesh.vertices[j];
                    vert /= 4;
                    vert += new Vector3(0.5f, 0.5f, 0.5f);
                    if (vert.x < 0f || vert.y < 0f || vert.z < 0f)
                        Debug.Log(vert);
                    positions.SetPixel(j, i, new Color(vert.x, vert.y, vert.z));
                }
            }

            var positionsData = positions.EncodeToPNG();
            File.WriteAllBytes(Path.Combine("Assets", positions.name + ".png"), positionsData);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

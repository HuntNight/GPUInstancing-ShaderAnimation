using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Instancer : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private uint _count;
    [SerializeField] private float _spread;

    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _positionsBuffer;
    private uint[] _args = { 0,0,0,0,0 };

    private void Start()
    {
        _argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        OnValidate();
    }

    private void OnValidate()
    {
        UpdateArgs();
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        if (_positionsBuffer != null)
            _positionsBuffer.Release();
        _positionsBuffer = new ComputeBuffer((int) _count, 12, ComputeBufferType.Structured);

        List<Vector3> positions = new List<Vector3>((int) _count);

        for (int i = 0; i < _count; i++)
        {
            positions.Add(new Vector3(Random.Range(-_spread, _spread), 0, Random.Range(-_spread, _spread)));
        }
        _positionsBuffer.SetData(positions);
        _material.SetBuffer("_Positions", _positionsBuffer);
    }

    private void UpdateArgs()
    {
        _args[0] = _mesh.GetIndexCount(0);
        _args[1] = _count;
        _args[2] = _mesh.GetIndexStart(0);
        _args[3] = _mesh.GetBaseVertex(0);
        if (_argsBuffer == null)
            return;
        _argsBuffer.SetData(_args);
    }

    private void Update()
    {
        Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, new Bounds(Vector3.zero, new Vector3(100, 100, 100)), _argsBuffer);
    }
}

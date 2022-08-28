Shader "Custom/Instanced" 
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"
        _PositionsTex("PositionsTex", 2D) = "white"
        _Length ("Length", float) = 1
    }
    SubShader 
    {
        Pass 
        {
            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag;

            struct v2f
            {
                float4 position : SV_Position;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex, _PositionsTex;
            StructuredBuffer<float3> _Positions;
            float4 _PositionsTex_TexelSize;
            float _Length;


            float3 RotateX(float3 vertex, float angle)
            {
                float alpha = angle * UNITY_PI / 180;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.yz), vertex.x).zxy;
            }
            

            v2f vert (appdata_full i, uint instanceId : SV_InstanceID, uint vertexId : SV_VertexID)
            {
                float x = (vertexId + 0.5) * _PositionsTex_TexelSize.x;
                float y = fmod(_Time.y / _Length, 1);
                float3 worldPosition = _Positions[instanceId];
                float3 localPosition = tex2Dlod(_PositionsTex, float4(x, y, 0, 0)) * 4 - float3(0.5, 0.5, 0.5);
                localPosition = RotateX(localPosition, 90);
                worldPosition += localPosition;
                v2f o;
                o.uv = i.texcoord;
                o.position = mul(UNITY_MATRIX_VP, float4(worldPosition, 1));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            
            ENDCG
        }
    }
}
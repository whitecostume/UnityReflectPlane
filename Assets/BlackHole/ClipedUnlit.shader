Shader "Unlit/ClipedUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS: TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _ClipPlane_Pos;
            float3 _ClipPlane_Nor;

            v2f vert (appdata v)
            {
                v2f o;
                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.vertex = TransformWorldToHClip(positionWS);
                o.positionWS = positionWS;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                // clip plane
                float3 vDir = i.positionWS - _ClipPlane_Pos;
                float3 vNor = _ClipPlane_Nor;
                float fDot = dot(vDir, vNor);
                clip(fDot);

                return col;
            }
            ENDHLSL
        }
    }
}

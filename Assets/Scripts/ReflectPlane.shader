Shader "Unlit/ReflectPlane"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ReflectTex("Reflect Texture", 2D) = "white" {}
        _FresnelParam("Fresnel",  Vector) = (0, 1, 1, 0.05)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ReflectTex;
            float4 _FresnelParam;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                screenUV = float2(1-screenUV.x,screenUV.y);
                

                float3 V = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
                float3 N = i.normal;
                float NV = dot(N,-V);
                float fresnel = 1.0 - NV;
                fresnel = _FresnelParam.x + (_FresnelParam.y - _FresnelParam.x) * pow(fresnel,_FresnelParam.z);
                fresnel *= _FresnelParam.w;
                float4 reflectColor = tex2D(_ReflectTex, screenUV);
                if (NV <= 0)
                {
                    
                }
                else
                {
                    col  = lerp(col,reflectColor,fresnel) ;
                }
                
                return col;
            }
            ENDCG
        }
    }
}

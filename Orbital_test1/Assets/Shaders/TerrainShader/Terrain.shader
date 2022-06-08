Shader "Unlit/Terrain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        struct VertexInput
        {
            float4 position : POSITION;
            float3 norm : NORMAL;
            //float2 uv : TEXCOO RD0;
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
            float3 norm : TEXCOORD0;
            //float2 uv : TEXCOORD0;
        };

        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float rand_1_05(in float2 uv)
            {
                float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233) * 2.0)) * 43758.5453));
                return abs(noise.x + noise.y) * 0.5;
            }

            VertexOutput vert(VertexInput i) 
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.norm = i.norm;
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                //float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 c = 0;
                c.rgb = i.norm * 0.5 + 0.5;
                return c;
            }

            ENDHLSL
        }
    }
}

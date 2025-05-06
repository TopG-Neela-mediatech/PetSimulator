Shader "Custom/TeethWithFoamFade_URP"
{
    Properties
    {
        _MainTex ("Dirty Texture", 2D) = "white" {}
        _CleanTex ("Clean Texture", 2D) = "white" {}
        _DirtMask ("Dirt Mask", 2D) = "white" {}
        _FoamTex ("Foam Texture", 2D) = "white" {}
        _FoamMask ("Foam Mask", 2D) = "white" {}
        _ColorTint ("Foam Tint", Color) = (1,1,1,1)
        _FoamFade ("Foam Fade 0-1", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);       SAMPLER(sampler_MainTex);
            TEXTURE2D(_CleanTex);      SAMPLER(sampler_CleanTex);
            TEXTURE2D(_DirtMask);      SAMPLER(sampler_DirtMask);
            TEXTURE2D(_FoamTex);       SAMPLER(sampler_FoamTex);
            TEXTURE2D(_FoamMask);      SAMPLER(sampler_FoamMask);

            float4 _ColorTint;
            float _FoamFade; // 0 = full, 1 = gone

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                float4 dirty = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                float4 clean = SAMPLE_TEXTURE2D(_CleanTex, sampler_CleanTex, uv);
                float dirtMask = SAMPLE_TEXTURE2D(_DirtMask, sampler_DirtMask, uv).r;

                float4 surface = lerp(dirty, clean, dirtMask);

                float foamMask = SAMPLE_TEXTURE2D(_FoamMask, sampler_FoamMask, uv).r;
                float4 foam = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, uv) * _ColorTint;

                // Fade logic: multiply alpha by (1 - _FoamFade)
                foam.a *= foamMask * (1.0 - _FoamFade);

                return lerp(surface, foam, foam.a);
            }

            ENDHLSL
        }
    }
}

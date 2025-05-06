Shader "Custom/BrushPainter_TextureStampURP"
{
    Properties
    {
        _BaseColor ("Brush Color", Color) = (1,1,1,1)
        _BrushTex ("Brush Texture", 2D) = "white" {}
        _Radius ("Brush Radius (0-0.5)", Float) = 0.45
        _Feather ("Feather (soft edge)", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Name "BrushPass"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BrushTex);
            SAMPLER(sampler_BrushTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            float4 _BaseColor;
            float _Radius;
            float _Feather;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(input.uv, center);
                float radialAlpha = smoothstep(_Radius, _Radius - _Feather, dist);

                float brushSample = SAMPLE_TEXTURE2D(_BrushTex, sampler_BrushTex, input.uv).r;

                float alpha = radialAlpha * brushSample;

                return float4(_BaseColor.rgb, _BaseColor.a * alpha);
            }
            ENDHLSL
        }
    }
}

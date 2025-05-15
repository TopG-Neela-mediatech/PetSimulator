Shader "Custom/FoamBubbleTextured"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 0.4)
        _FresnelPower ("Fresnel Power", Range(0.01, 1)) = 0.3
        _Alpha ("Alpha", Range(0, 1)) = 0.4
        _GlowStrength ("Edge Glow Strength", Range(0.5, 2)) = 1.2
        _FoamTex ("Foam Texture", 2D) = "white" {}
        _FoamTiling ("Foam Tiling", Range(0.5, 5)) = 1.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _FoamTex;
            float _FoamTiling;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldViewDir : TEXCOORD1;
                float2 uv : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float _FresnelPower;
            float _Alpha;
            float _GlowStrength;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldViewDir = _WorldSpaceCameraPos.xyz - worldPos;
                o.uv = v.uv * _FoamTiling;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.worldViewDir);
                float fresnel = pow(1.0 - dot(normal, viewDir), _FresnelPower);

                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);

                float3 edgeGlow = baseColor.rgb * _GlowStrength;
                baseColor.rgb = lerp(baseColor.rgb, edgeGlow, fresnel);

                // Sample foam texture and use as additional soft alpha mask
                float foamMask = tex2D(_FoamTex, i.uv).r;
                baseColor.a = lerp(0.2, _Alpha, fresnel) * foamMask;

                return baseColor;
            }
            ENDCG
        }
    }

    FallBack Off
}

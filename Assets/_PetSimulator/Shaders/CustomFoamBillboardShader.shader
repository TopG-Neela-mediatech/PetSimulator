Shader "Custom/SphereBubble"
{
    Properties
    {
        _BaseColor ("Fallback Color", Color) = (1, 1, 1, 1)
        _Alpha ("Base Alpha", Range(0,1)) = 0.5
        _FresnelPower ("Fresnel Power", Range(0, 8)) = 3.0
        _GlowStrength ("Glow Strength", Range(0, 2)) = 1.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Back
        LOD 200
        AlphaTest Greater 0.01

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            float _Alpha;
            float _FresnelPower;
            float _GlowStrength;
            float4 _BaseColor;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                float4 color = instanceColor * _BaseColor;

                float fresnel = pow(1.0 - saturate(dot(normalize(i.normal), normalize(i.viewDir))), _FresnelPower);
                float pulse = 0.5 + 0.5 * sin(_Time.y * 3.0);
                float coreLight = pow(saturate(dot(i.normal, i.viewDir)), 4.0);

                color.rgb += fresnel * _GlowStrength;
                color.rgb += coreLight * 0.2;
                color.rgb += pulse * 0.1; // softly increase brightness on pulse
                color.a = fresnel * _Alpha * color.a;

                return color;
            }
            ENDCG
        }
    }

    FallBack Off
}

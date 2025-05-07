Shader "Custom/SoftFoamUnlit"
{
    Properties
    {
        _MainTex ("Foam Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Softness ("Edge Softness", Range(0.01, 2)) = 0.5
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            Tags { "LightMode"="Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Softness;
            float _Alpha;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

                float alpha = texColor.a;
                alpha = smoothstep(1.0, _Softness, alpha); // soften edges
                texColor.rgb *= color.rgb;
                texColor.a = alpha * _Alpha * color.a;

                return texColor;
            }
            ENDCG
        }
    }
}

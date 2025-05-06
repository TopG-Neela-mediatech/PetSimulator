Shader "Custom/DrawBrush"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _UV ("Brush UV", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _UV;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 delta = i.uv - _UV.xy;
                float dist = length(delta);
                float brush = smoothstep(_UV.z, _UV.z * 0.8, dist);
                float mask = tex2D(_MainTex, i.uv).r;
                mask = lerp(1, mask, brush);
                return fixed4(mask, mask, mask, 1);
            }
            ENDCG
        }
    }
}

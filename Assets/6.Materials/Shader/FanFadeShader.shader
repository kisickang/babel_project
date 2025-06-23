Shader "Custom/FanFadeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.7, 0.3, 1)
        _Progress ("Progress", Range(0, 1)) = 0
        _BandSize ("Band Size", Float) = 5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Progress;
            float _BandSize;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 localPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(i.localPos);
                float fade = saturate(1 - (_Progress - dist / _BandSize) * 10);

                return fixed4(_Color.rgb, _Color.a * fade);
            }
            ENDCG
        }
    }
}

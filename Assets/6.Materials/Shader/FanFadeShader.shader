Shader "Custom/FanFadeShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 0.5, 0, 1)
        _Progress("Progress", Range(0,1)) = 0
        _BandSize("Band Size", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = i.uv.y;
                float start = _Progress;
                float end = _Progress + (_BandSize * 0.01); // 0~1 정규화

                float visible = step(start, dist) * (1.0 - step(end, dist));
                fixed4 col = _Color;
                col.a *= visible;
                return col;
            }
            ENDCG
        }
    }
}

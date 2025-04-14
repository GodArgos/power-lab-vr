// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable

Shader "Custom/AbyssFade"
{
    Properties
    {
        _MainColor ("Base Color", Color) = (0.1, 0.1, 0.1, 1)
        _FadeDistance ("Fade Start Distance", Float) = 20
        _FadeStrength ("Fade Strength", Float) = 2
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _MainColor;
            float _FadeDistance;
            float _FadeStrength;
            // float3 _WorldSpaceCameraPos;

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _WorldSpaceCameraPos);
                float fade = saturate((dist - _FadeDistance) / _FadeStrength);
                return lerp(_MainColor, float4(0,0,0,1), fade);
            }
            ENDCG
        }
    }
}

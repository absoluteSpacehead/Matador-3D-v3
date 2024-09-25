Shader "Hidden/AnalogVideo/CRT"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "black" {}
        _ShadowMask ("Shadow Mask", 2D) = "white" {}
        _ShadowMaskAmt ("Shadow Mask Amount", Range(0, 1)) = 0.5
        _HorizontalCurvature ("Horizontal Curvature", Range(0, 1)) = 1
        _VerticalCurvature ("Vertical Curvature", Range(0, 1)) = 1
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Off

        CGINCLUDE
            #include "UnityCG.cginc"

            #define pi 3.14159265359

            sampler2D _MainTex;
            sampler2D _ShadowMask;
            float4 _ShadowMask_ST;

            float _ShadowMaskAmt;

            float _HorizontalCurvature;
            float _VerticalCurvature;

            float2 _CRTResolution;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
        ENDCG

        // Pass 0
        // Shadow mask
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            float4 frag (v2f i) : SV_Target
            {
                float3 mask = lerp(1, tex2Dlod(_ShadowMask, float4(i.uv * _CRTResolution * _ShadowMask_ST.xy + _ShadowMask_ST.zw, 0, 0)).rgb, _ShadowMaskAmt);

                //i.uv = (floor(i.uv * _CRTResolution) + 0.5) / _CRTResolution;

                float4 col = tex2Dlod(_MainTex, float4(i.uv, 0, 0));
                col.rgb *= mask;

                return col;
            }
            ENDCG
        }

        // Pass 1
        // Curvature
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            float2 Curvature(float2 uv)
            {
                float2 centeredUv = (uv - 0.5);

                float2 curvedUv = centeredUv.xy / cos( centeredUv.yx * pi/2.0 ) + 0.5;

                uv.x = lerp(uv.x, curvedUv.x, _VerticalCurvature);
                uv.y = lerp(uv.y, curvedUv.y, _HorizontalCurvature);

                return uv;
            }
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = Curvature(i.uv);

                // Prevent aliasing on the edges
                float maxDist = max(abs(uv.x - 0.5), abs(uv.y - 0.5)) * 2;
                float edge = 1 - smoothstep(0.995, 1, maxDist);

                return tex2Dlod(_MainTex, float4(uv, 0, 0)) * edge;
            }
            ENDCG
        }
    }
}

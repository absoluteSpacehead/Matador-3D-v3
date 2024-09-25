Shader "Hidden/sharpen"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Amount("Amount",Float) = 0.2
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Amount;

			fixed4 sharpen(sampler2D tex, float2 coords) {
				float dx = 1.0 / 640;
				float dy = 1.0 / 480;
				fixed4 sum = 5 * tex2D(tex, coords);
				sum += -tex2D(tex, coords + float2(-dx, 0));
				sum += -tex2D(tex, coords + float2( dx, 0));
				sum += -tex2D(tex, coords + float2(0, -dy));
				sum += -tex2D(tex, coords + float2(0,  dy));
				return sum;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                //TO JAMES: try to unify this with the ntsc and youll face performance hell. keep it separate pls.
                return sharpen(_MainTex, i.uv) * _Amount + tex2D(_MainTex, i.uv) * (1 - _Amount);
            }
            ENDCG
        }
    }
}

Shader "Hidden/AnalogVideo/Main"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "black" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Off

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "AnalogVideo.cginc"

		#define BLUR (4)
		#define SCANLINE (0.01)

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
		ENDCG

		// Pass 0
		// Analog encode
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _Resolution;

			float Dither(float2 uv, float val, float quantize)
			{
				float2 pos = floor((1 - uv.xy) * _Resolution.xy);

				float pattern = ((pos.x + pos.y) % 2) / 2 - 0.25;

				return round(val * quantize + pattern) / max(quantize, 1);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 displayRes = _MainTex_TexelSize.zw;

				float2 uv = i.uv;
				uv.y = (floor(uv.y * _OutputRes.y) + 0.5) / _OutputRes.y; // Quantize UV Y to scanlines
				
				float4 col = tex2D(_MainTex, uv);


				// Apply 7-bit checkerboard dithering to cause vertical color banding
				const float bits = 7;
				const float quantize = pow(2, bits) - 1;

				col.r = Dither(uv, col.r, quantize);
				col.g = Dither(uv, col.g, quantize);
				col.b = Dither(uv, col.b, quantize);


				col.rgb = Encode(col.rgb, i.uv);

				return col;
			}
			ENDCG
		}

		// Pass 1
		// Analog decode
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = 0;
				col.rgb = Decode(i.uv);
				return col;
			}
			ENDCG
		}

		// Pass 2
		// Final
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float3 Sample(float2 uv)
			{
				return tex2Dlod(_MainTex, float4(uv, 0, 0)).rgb
					* (1 - abs(floor(uv.x))); // Return 0 when sampling outside of [0,1]
			}
			float3 HorizontalBlur(float2 uv, float range)
			{
				const int samples = 16;

				float3 sum = 0;
				float weight = 0;

				range *= 1 / CHROMA_FREQ;

				float x = -range/2;
				float dx = range / samples;

				[unroll]
				for(int i = 0; i < samples; i++)
				{
					float w = 1 - abs(x / range);

					sum += Sample(float2(uv.x + x, uv.y)) * w;
					weight += w;

					x += dx;
				}

				return sum / weight;
			}

			float3 Sharpen(float3 yiq, float2 uv)
			{
				const float sharpenAmt = 0.25;

                // Sharpening
				float dx = 1.0 / 227.5;
				float sharpen = yiq.x * 1e-10;
				float sumWeight = 1e-10;

				float3 sum = 0;

				const int kernelSize = 1;
				for(int i = 1; i <= kernelSize; i++)
				{
					float weight = 1.0 / i;

					sum += tex2Dlod(_MainTex, float4(uv.x + i*dx, uv.y, 0, 0)) * weight;
					sum += tex2Dlod(_MainTex, float4(uv.x - i*dx, uv.y, 0, 0)) * weight;

					sumWeight += weight * 2;
				}
				sharpen = RGBtoYIQ(sum / sumWeight).x;

				yiq.x = lerp(yiq.x, yiq.x * 2 - sharpen, sharpenAmt);

				return yiq;
			}

			float ApplyScanlines(float luma, float2 uv)
			{
				const float2 res = float2(320, 240);

				float sl = floor(fmod(uv.y * res.y, 2));
				//sl *= saturate(floor(fmod(uv.x * res.x / 2, 2)) + floor(fmod(uv.y * res.y / 2, 2))); // Segmented scanlines

				luma *= 1.0 + sl * SCANLINE;

				return luma;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				const float2 res = float2(320, 240);

				i.uv.y = (floor(i.uv.y * res.y) + 0.5) / res.y;

				float4 col = tex2Dlod(_MainTex, float4(i.uv, 0, 0));

				float3 yiq = RGBtoYIQ(col.rgb);

				float3 blurred = RGBtoYIQ(HorizontalBlur(i.uv, BLUR));
				yiq.x = Sharpen(yiq, i.uv);
				yiq.yz = blurred.yz;
				
				yiq.x = ApplyScanlines(yiq.x, i.uv);

				col.rgb = YIQtoRGB(yiq);

				return col;
			}
			ENDCG
		}
	}
}

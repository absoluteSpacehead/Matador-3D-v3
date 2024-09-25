Shader "Petscop NTSC"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		HorizontalBlur ("Horizontal Blur", Range(0,0.005)) = 0.0004
		ColorBlur ("Color Blur", Range(0,0.005)) = 0.0025
		DistAmount ("Distortion Amount", Range(0,0.05)) = 0.01
		[PerRendererData] DistInvert ("Distortion Invert", Float) = 1
		SLAmount ("Scanline Amount", Range(0,1)) = 0.03
		BandingAmount ("Banding Amount", Range(0, 1)) = 0.1
		FringeMultiplier ("Fringe Multiplier", Range(0, 1)) = 1
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
			float HorizontalBlur;
			float ColorBlur;
			float DistAmount;
			float SLAmount;
			float DistInvert;
			float BandingAmount;
			float BandingWidth;
			float FringeMultiplier;

			#define pi 3.14159265359

			fixed4 blurify(float2 uv, sampler2D image, float size){
				float2 tc0 = uv + float2(      0, 0.0);
				float2 tc1 = uv + float2( size  , 0.0);
				float2 tc2 = uv + float2( size*2, 0.0);
				float2 tc3 = uv + float2( size*3, 0.0);
				float2 tc4 = uv + float2( size*4, 0.0);
				float2 tc5 = uv + float2(-size  , 0.0);
				float2 tc6 = uv + float2(-size*2, 0.0);
				float2 tc7 = uv + float2(-size*3, 0.0);
				float2 tc8 = uv + float2(-size*4, 0.0);

				fixed4 col0 = tex2D(image, tc0);
				fixed4 col1 = tex2D(image, tc1);
				fixed4 col2 = tex2D(image, tc2);
				fixed4 col3 = tex2D(image, tc3);
				fixed4 col4 = tex2D(image, tc4);
				fixed4 col5 = tex2D(image, tc5);
				fixed4 col6 = tex2D(image, tc6);
				fixed4 col7 = tex2D(image, tc7);
				fixed4 col8 = tex2D(image, tc8);

				fixed4 sum = (1.0 * col0 + 1.0 * col1 + 1.0 * col2 +
				              1.0 * col3 + 1.0 * col4 + 1.0 * col5 +
				              1.0 * col6 + 1.0 * col7 + 1.0 * col8) / 9.0;

				return sum;
			}

			float bandchannel(float uv){
				float i = uv % 3;
				if(i > 2) return -0.25;
				return 0.25 - cos(i*pi)*0.5;
			}

			fixed4 GetBand(float uv)
			{
				uv = (uv * 3 + DistInvert + 1) * 10; //counted, its 10
				return fixed4(bandchannel(uv), bandchannel(uv + 1), bandchannel(uv + 2),0);
			}

			float GetEdge(float uv)
			{
				if(uv < 0.5)
					return min(uv * 300, 1);
				else
					return min((1 - uv) * 300, 1);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				//  >>-------------Squiggle Map Calculation-------------<<
				float squiggle = floor(i.uv.y * 240) % 2 - 0.5;

				//  >>-------------Fringing-------------<<
				fixed3 col = blurify(i.uv, _MainTex, HorizontalBlur) * GetEdge(i.uv.x); //expensive but prettier way
				//fixed3 col = tex2D(_MainTex, i.uv) * GetEdge(i.uv.x);                   //faster but uglier way
				float2 pos = i.uv + float2(squiggle * (col.r - col.g) * DistAmount * DistInvert, 0);

				//  >>-------------Color Blur-------------<<
				fixed horblr = Luminance(blurify(pos, _MainTex, HorizontalBlur).rgb);
				fixed3 colblr = blurify(pos, _MainTex, ColorBlur).rgb;
				colblr = (colblr - Luminance(colblr));
				col = colblr + horblr;
				
				//  >>-------------Color Banding-------------<<
				col = GetBand(i.uv) * (abs(colblr.r) + abs(colblr.g) + abs(colblr.b)) * BandingAmount + colblr * (1 - BandingAmount) + horblr;
				
				//  >>-------------Scanlines-------------<<
				if(horblr > 0.5)
					col = 1 - ((1 - col) * (1 - squiggle * SLAmount));
				else 
					col *= (1 + squiggle * SLAmount);

				//  >>-------------Thank God We're Done (said the gpu to the motherboard)-------------<<
				return fixed4(col, 1) * GetEdge(pos.x);
			}
			ENDCG
		}
	}
}

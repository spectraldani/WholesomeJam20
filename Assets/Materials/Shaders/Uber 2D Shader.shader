Shader "Custom/Uber 2D Shader"
{
	Properties
	{
		[PerRendererData]_MainTex ("Texture", 2D) = "white" {}

		[Header(Palette Swap)]
		_PaletteColor0("Palette Color 0 - Highlight", Color) = (1,1,1,1)
		_PaletteColor1("Palette Color 1 - Light", Color) = (0.75,0.75,0.75,1)
		_PaletteColor2("Palette Color 2 - Shadow", Color) = (0.5,0.5,0.5,1)
		_PaletteColor3("Palette Color 3 - Outlines", Color) = (0,0,0,1)
		_PaletteSwapStrength("Mix Level", Range(0,1)) = 1.0

		[Header(Hatching)]
		_HatchTex1("Hatching Texture 1 (SDF)", 2D) = "white" {}
		_HatchTex2("Hatching Texture 2 (SDF)", 2D) = "white" {}
		_PaperTex ("Paper Texture", 2D) = "white" {}
		_HatchWeight("Hatch Weight", Range(1, 2)) = 1.1
		[Space]
		_RemapInputMin("Remap Input -  Min Value", Range(0, 1)) = 0
		_RemapInputMax("Remap Input -  Max Value", Range(0, 1)) = 1
		[Space]
		_RemapOutputMin("Remap Output - Min Value", Range(0, 1)) = 0
		_RemapOutputMax("Remap Output - Max Value", Range(0, 1)) = 1
		[Space]
		_LightColor("Light Color", Color) = (1,1,1,1)
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
		_HatchingStrength("Mix Level", Range(0,1)) = 0.5

		[Header(Outline)]
		_OutlineSize ("Size", Range (0, 2))  = 1.0
		_OutlineColor("Color", COLOR) = (1,1,1,1)
		_OutlineHDR("HDR", Range(1, 2)) = 1

		[Header(Doodle)]
		_NoiseScale("Noise Intensity", Range(0,0.5)) = 1
		_FPS("Frames Per Second", Range(1,60)) = 8
	}
	SubShader
	{
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 screenPos  : TEXCOORD1;
			};
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			
			//HELPER FUNCTIONS:

			float Epsilon = 1e-10;
			float3 RGBtoHCV(in float3 RGB)
			{
				// Based on work by Sam Hocevar and Emil Persson
				float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
				float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
				float C = Q.x - min(Q.w, Q.y);
				float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
				return float3(H, C, Q.x);
			}



			//PALETTE SWAP
			float4 _PaletteColor0,_PaletteColor1, _PaletteColor2, _PaletteColor3;
			float _PaletteSwapStrength;


			float4 areaSample(float2 texcoord) {

				float4 result = tex2D(_MainTex, texcoord);
				result += tex2D( _MainTex, texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y));
				result += tex2D(_MainTex, texcoord + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y));
				result += tex2D(_MainTex, texcoord + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y));
				result += tex2D(_MainTex, texcoord + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.x));
				return result /5;
			}

			void kernels(inout float4 n[9], float2 texcoord) {

				float2 size = _MainTex_TexelSize;

				n[0] = tex2D(_MainTex, texcoord + float2(-size.x, -size.y));
				n[1] = tex2D(_MainTex, texcoord + float2(0, -size.y));
				n[2] = tex2D(_MainTex, texcoord + float2(size.x, -size.y));	
				n[3] = tex2D(_MainTex, texcoord + float2( -size.x, 0));
				n[4] = tex2D(_MainTex, texcoord);
				n[5] = tex2D(_MainTex, texcoord + float2(size.x, 0));
				n[6] = tex2D(_MainTex, texcoord + float2(-size.x, size.y));
				n[7] = tex2D(_MainTex, texcoord + float2(0, size.y));
				n[8] = tex2D(_MainTex, texcoord + float2(size.x,   size.y));
			}

			/*float4 Sobel(float2 texcoord) {
				float4 n[9];
				kernels(n, texcoord);

				float4 sobel_edge_h = n[2] + (2.0*n[5]) + n[8] - (n[0] + (2.0*n[3]) + n[6]);
				float4 sobel_edge_v = n[0] + (2.0*n[1]) + n[2] - (n[6] + (2.0*n[7]) + n[8]);
				float4 sobel = sqrt((sobel_edge_h * sobel_edge_h) + (sobel_edge_v * sobel_edge_v));
				return float4(1 - step(saturate(sobel.rgb), 0.9, 5), 1.0);
			}*/

			float4 PaletteSwapFX(float4 spriteColor, float2 texcoord) {
				float4 result = spriteColor;
				float interpolator = RGBtoHCV(spriteColor.rgb).b;
				//interpolator = spriteColor.r;
				result.rgb =
					lerp(_PaletteColor1, _PaletteColor0, (interpolator - 0.75) * 4.0) * step(0.75, interpolator)// HIGHLIGHT TO LIGHT
					+
					lerp(_PaletteColor2, _PaletteColor1, (interpolator - 0.25) * 2.0) * step(0.25, interpolator) * step(interpolator,0.75) // LIGHT TO SHADOW
					+
					lerp(_PaletteColor3, _PaletteColor2, (interpolator) * 4.0) * step(interpolator, 0.25); // SHADOW TO OUTLINE			

				return lerp(spriteColor,result,_PaletteSwapStrength);
			}


			//HATCH FX:
			sampler2D _HatchTex1;
			sampler2D _HatchTex2;
			sampler2D _PaperTex;
			float4 _PaperTex_ST;
			float4 _HatchTex1_ST;
			float4 _HatchTex2_ST;
			float _RemapInputMin;
			float _RemapInputMax;
			float _RemapOutputMin;
			float _RemapOutputMax;
			float _HatchWeight;
			fixed4 _ShadowColor;
			fixed4 _LightColor;
			fixed _HatchingStrength;

			float map(float input, float inMin, float inMax, float outMin, float outMax) {
				float relativeValue = (input - inMin) / (inMax - inMin);
				return lerp(outMin, outMax, relativeValue);
			}

			float4 HatchFX(v2f IN, float4 spriteColor) {
				if (_HatchingStrength == 0.0) return spriteColor;
				float2 screenPos = IN.screenPos.xy / IN.screenPos.w;
				float aspect = _ScreenParams.x / _ScreenParams.y;
				screenPos.x = screenPos.x * aspect;
				fixed4 hatch1 = tex2D(_HatchTex1, TRANSFORM_TEX(screenPos, _HatchTex1));
				fixed4 hatch2 = tex2D(_HatchTex2, TRANSFORM_TEX(screenPos, _HatchTex2));
				fixed4 paper = tex2D(_PaperTex, IN.texcoord);

				float brightness = RGBtoHCV(spriteColor.rgb).b;
				hatch1.a = map(hatch1.a, _RemapInputMin, _RemapInputMax, _RemapOutputMin, _RemapOutputMax);
				hatch2.a = map(hatch2.a, _RemapInputMin, _RemapInputMax, _RemapOutputMin, _RemapOutputMax);
				float hatch1Change = fwidth(hatch1.a) * 0.5;
				float hatch2Change = fwidth(hatch2.a) * 0.5;
				brightness = smoothstep((hatch1.a - hatch1Change), (hatch1.a + hatch1Change), brightness)  *  smoothstep((hatch2.a - hatch2Change), (hatch2.a + hatch2Change), brightness * _HatchWeight);
				float4 result = spriteColor;
				result.rgb = lerp(_ShadowColor.rgb, _LightColor.rgb, brightness);
				result.rgb = lerp(spriteColor.rgb, result.rgb, _HatchingStrength);
				result.rgb *= paper;
				return result;
			}

			//BLUR FX:
			float _BlurIntensity = 5.0f;
			float4 BlurFX(float2 uv, float4 spriteColor) {
				float stepU = 0.00390625f * 2;
				float stepV = stepU;
				float4 result = float4 (0, 0, 0, 0);
				float2 texCoord = float2(0, 0);
				texCoord = uv + float2(-stepU, -stepV); result += tex2D(_MainTex, texCoord);
				texCoord = uv + float2(-stepU, 0); result += 2.0 * tex2D(_MainTex, texCoord);
				texCoord = uv + float2(-stepU, stepV); result += tex2D(_MainTex, texCoord);
				texCoord = uv + float2(0, -stepV); result += 2.0 * tex2D(_MainTex, texCoord);
				texCoord = uv; result += 4.0 * tex2D(_MainTex, texCoord);
				texCoord = uv + float2(0, stepV); result += 2.0 * tex2D(_MainTex, texCoord);
				texCoord = uv + float2(stepU, -stepV); result += tex2D(_MainTex, texCoord);
				texCoord = uv + float2(stepU, 0); result += 2.0* tex2D(_MainTex, texCoord);
				texCoord = uv + float2(stepU, -stepV); result += tex2D(_MainTex, texCoord);
				result = result * 0.0625;
				return result;
			}

			//OUTLINE FX:

			float _OutlineSize;
			fixed4 _OutlineColor;
			float _OutlineHDR;

			float4 OutlineFX(float2 uv, float4 spriteColor)
			{
				if (_OutlineSize == 0) return spriteColor;
				float value = _OutlineSize * 0.01;
				float4 mainColor = tex2D(_MainTex, uv + float2(-value, value))
					+ tex2D(_MainTex, uv + float2(value, -value))
					+ tex2D(_MainTex, uv + float2(value, value))
					+ tex2D(_MainTex, uv - float2(value, value));

				_OutlineColor *= _OutlineHDR;
				mainColor.rgb = _OutlineColor.rgb;
				float4 addcolor = spriteColor;
				if (mainColor.a > 0.40) {
					mainColor = _OutlineColor;
				}
				if (addcolor.a > 0.40) {
				mainColor = addcolor;
				mainColor.a = addcolor.a;
				}
				return mainColor;
			}

			//DOODLE FX
			float _NoiseScale;
			float _FPS;

			inline float3 random3(float3 c) {
				float j = 4096.0*sin(dot(c, float3(17.0, 59.4, 15.0)));
				float3 r;
				r.z = frac(512.0*j);
				j *= .125;
				r.x = frac(512.0*j);
				j *= .125;
				r.y = frac(512.0*j);
				return r - 0.5;
			}

			float snapToFPS(float time, float fps)
			{
				return round(time * fps) * fps;
			}
			float4 DoodleFX(float4 vertexObjSpace, float4 vertex) {

				float time = snapToFPS(_Time.y, _FPS);
				//float2 noise = random3(vertex.xyz + float3(time, 0.0, 0.0)).xy * _NoiseScale;
				float2 noise = random3(vertexObjSpace.xyz + float3(time, 0.0, 0.0)).xy * _NoiseScale;
				//Divide noise by camera size to avoid
				vertex.xy += noise / unity_OrthoParams.x;

				return vertex;

			}

			v2f vert (appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				//DOODLE FX:
				OUT.vertex = DoodleFX(IN.vertex, OUT.vertex) ;

				OUT.screenPos = ComputeScreenPos(OUT.vertex);
				return OUT;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				fixed4 sprite = tex2D(_MainTex, IN.texcoord);
				fixed4 finalColor = sprite;

				//BLUR FX
				//float4 blur = BlurFX(IN.texcoord, finalColor);
				//finalColor = blur;

				//PALETTE SWAP
				float4 paletteSwap = PaletteSwapFX(finalColor, IN.texcoord);	
				finalColor = paletteSwap;

				//HATCH FX
				float4 hatch = HatchFX(IN, finalColor);
				finalColor = hatch;

				finalColor.rgb *= IN.color.rgb;

				//OUTLINE FX
				float4 outline = OutlineFX(IN.texcoord, finalColor);
				finalColor = outline;


				finalColor.rgb *= finalColor.a;
				return finalColor;
			}
			ENDCG
		}
	}
}

Shader "Toybox/DisplayNumber2"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Number ("Number", Float) = 0.0
		_Columns ("Atlas Columns", Float) = 4
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#define MAX_DIGITS 7

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 tangent : TANGENT;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float fogCoord : TEXCOORD1;
				float4 tangent : TANGENT;
			};
			
			struct g2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float fogCoord : TEXCOORD1;
			};

			float _Number;
			float _Columns;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2g vert (appdata v)
			{
				v2g o;
				o.vertex = v.vertex;
				o.tangent = v.tangent;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.fogCoord = o.vertex.z;
				return o;
			}
			
			[maxvertexcount(MAX_DIGITS * 3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> outStream, uint pid : SV_PrimitiveID)
			{
				float unit = (1.0f / MAX_DIGITS);
				float number = floor(max(_Number, 0.0f));
				float digits = min(1 + floor(log10(max(number, 1.0f))), MAX_DIGITS);
				float4 tangent = input[0].tangent;

				float columns = floor(max(1.0, _Columns));
				float cellSize = 1.0 / columns;

				
				for (int i = 0; i < digits; ++i)
				{
					float pow10 = pow(10, i);
					float digit = floor(fmod((number + 0.1) / pow10, 10));
					float x = fmod(digit, columns);
					float y = 3 - floor(digit / columns);					
					
					for (int j = 0; j < 3; ++j)
					{
						float4 vtx = input[j].vertex;
						float2 uv = input[j].uv;
						//vtx = vtx + tangent * (-uv.x + unit * uv.x + unit * (digits - i - 1) - 0.5f * digits * unit + 0.5f);
						vtx = vtx + tangent * (-uv.x + 0.5f + unit * (uv.x - i - 1 + 0.5f * digits));

						g2f vertex;
						vertex.position = UnityObjectToClipPos(vtx);
						vertex.fogCoord = input[j].fogCoord;
						vertex.uv.x = (input[j].uv.x + x) * cellSize;
						vertex.uv.y = (input[j].uv.y + y) * cellSize;
						
						outStream.Append(vertex);
					}
					outStream.RestartStrip();
				}
			}
			
			
			fixed4 frag (g2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				if (col.a < 0.1)
					discard;
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

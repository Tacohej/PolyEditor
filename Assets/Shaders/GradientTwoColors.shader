﻿
Shader "Unlit/GradientTwoColors"
{
	Properties
	{
		// _MainTex ("Texture", 2D) = "white" {}
		_TopColor ("Top Color", COLOR) = (1,1,1,1)
		_BottomColor ("Bottom Color", COLOR) = (0,0,0,1)
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
			// make fog work
			// #pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 w_pos : TEXCOORD1;
				float3 screen_pos: TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			// sampler2D _MainTex;
			// float4 _MainTex_ST;
			float4 _TopColor;
			float4 _BottomColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.w_pos = mul(unity_ObjectToWorld, v.vertex);
				o.screen_pos = ComputeScreenPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);

				float weight = clamp(0, 1, i.screen_pos.y/50);
				float3 pos = i.vertex;
				// pos = i.w_pos;


				fixed4 col = fixed4(pos, 1);
				//col = fixed4(i.screen_pos.x, i.screen_pos.y, i.screen_pos.z, 1);
				
				
				//fixed4 col = lerp(_BottomColor, _TopColor, weight);
				return col;
			}
			ENDCG
		}
	}
}

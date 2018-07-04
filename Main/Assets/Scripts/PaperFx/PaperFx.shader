Shader "Hidden/PaperFx"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PaperTex ("Paper Texture", 2D) = "white" {}
		_PaperOffset( "Paper Offset", Vector ) = ( 0,0,0,0 )
		_PaperScale( "Paper Scale", Vector ) = ( 0,0,0,0 )
		_PaperBlend( "Paper Scale", Float ) = 1
		_PaperDistort( "Paper Distortion", Float ) = 0.01
	}
	SubShader
	{
		// No culling or depth
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex, _PaperTex;
			float2 _PaperOffset, _PaperScale;
			float _PaperBlend, _PaperDistort;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 pco = tex2D( _PaperTex, ( i.uv + _PaperOffset ) * _PaperScale );
				pco = pco * pow( pco, _PaperBlend );

				fixed4 mco = tex2D( _MainTex, i.uv + ( (pco*2-1) * _PaperDistort ) );
				return mco * pco;
			}
			ENDCG
		}
	}
}

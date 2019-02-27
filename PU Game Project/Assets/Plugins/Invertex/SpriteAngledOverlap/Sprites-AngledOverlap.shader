// Height modulated sprite clipping shader to prevent the tops of tilted sprites from penetrating into geometry.
// -Invertex / invertex.xyz
Shader "Invertex/Sprites/Sprite_AngledOverlap" 
{
	Properties 
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint / Transparency", Color) = (1,1,1,1)
		_CullValues("Cull Values (null, bottom world pos, cull power, height)", Vector) = (1,1,1,1)
		
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}
	SubShader 
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		 	Cull Off
			Lighting Off
			ZWrite Off
			ZTest Off
			Blend SrcAlpha OneMinusSrcAlpha

	    Pass 
	    {

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		#pragma glsl
		#pragma multi_compile _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

		#include "UnityCG.cginc"
		#include "UnitySprites.cginc"

			uniform sampler2D _CameraDepthTexture;

			struct v2f_AngledOverlap
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				float vDepth : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};
			
			UNITY_INSTANCING_BUFFER_START(Props)
				float4 _CullValues;
			UNITY_INSTANCING_BUFFER_END(Props)

			inline float Remap(float value, float from1, float to1, float from2, float to2)
			{
				return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
			}

			v2f_AngledOverlap vert (appdata_t v)
			{
				v2f_AngledOverlap o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f_AngledOverlap, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
				o.vertex = UnityFlipSprite(v.vertex, _Flip);
				o.vertex = UnityObjectToClipPos(o.vertex);
				o.texcoord = v.texcoord;

				o.color = v.color * _Color * _RendererColor;
			#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap(o.vertex);
			#endif

				//Depth adjust stuff
				o.screenPos = ComputeScreenPos(o.vertex);
				float worldHeight = mul(unity_ObjectToWorld, v.vertex).y;
				float relWorldHeight = distance(worldHeight, _CullValues.y);
				relWorldHeight = Remap(relWorldHeight, 0, _CullValues.w, 0, 1) * _CullValues.z;
				COMPUTE_EYEDEPTH(o.vDepth);
				o.vDepth -= relWorldHeight;

				return o;
			}

			fixed4 frag(v2f_AngledOverlap i) : COLOR
			{
				float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
				depth = LinearEyeDepth(depth);
				depth =  depth - i.vDepth;
				clip(depth);
		
				float4 sprite = SampleSpriteTexture(i.texcoord) * i.color;
				//sprite.rgb *= sprite.a; (Made outlines around sprites)
				return sprite;
			}
		ENDCG
	    }
	} 
	Fallback "Sprites/Default"
}

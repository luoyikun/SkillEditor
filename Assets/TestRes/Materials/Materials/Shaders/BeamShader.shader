// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BeamShader"
{
	Properties
	{
		_Stepper("Stepper", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Emission("Emission", Float) = 0
		_FlowSpeed("FlowSpeed", Float) = 1
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_MaskTiling("MaskTiling", Vector) = (0,0,0,0)
		_MainTexTiling("MainTexTiling", Vector) = (0,0,0,0)
		_HighLightStepper("HighLightStepper", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _Emission;
		uniform sampler2D _TextureSample0;
		uniform float2 _MainTexTiling;
		uniform float _FlowSpeed;
		uniform float _HighLightStepper;
		uniform sampler2D _TextureSample1;
		uniform float2 _MaskTiling;
		uniform float _Stepper;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float4 temp_cast_6 = (_Stepper).xxxx;
			float2 uvs_TexCoord35 = i.uv_texcoord;
			uvs_TexCoord35.xy = i.uv_texcoord.xy * _MaskTiling;
			float4 uvs_TexCoord21 = i.uv_texcoord;
			uvs_TexCoord21.xy = i.uv_texcoord.xy * _MainTexTiling;
			float4 appendResult25 = (float4(0.0 , ( uvs_TexCoord21.z * -_FlowSpeed ) , 0.0 , 0.0));
			float4 tex2DNode32 = tex2D( _TextureSample1, ( float4( uvs_TexCoord35.xy, 0.0 , 0.0 ) + appendResult25 ).xy );
			c.rgb = 0;
			c.a = ( i.vertexColor.a * step( temp_cast_6 , tex2DNode32 ) ).r;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float4 uvs_TexCoord21 = i.uv_texcoord;
			uvs_TexCoord21.xy = i.uv_texcoord.xy * _MainTexTiling;
			float4 appendResult25 = (float4(0.0 , ( uvs_TexCoord21.z * -_FlowSpeed ) , 0.0 , 0.0));
			float4 temp_cast_1 = (_HighLightStepper).xxxx;
			float2 uvs_TexCoord35 = i.uv_texcoord;
			uvs_TexCoord35.xy = i.uv_texcoord.xy * _MaskTiling;
			float4 tex2DNode32 = tex2D( _TextureSample1, ( float4( uvs_TexCoord35.xy, 0.0 , 0.0 ) + appendResult25 ).xy );
			float4 temp_output_39_0 = step( temp_cast_1 , tex2DNode32 );
			float4 temp_cast_4 = (_HighLightStepper).xxxx;
			o.Emission = ( _Emission * ( ( i.vertexColor * tex2D( _TextureSample0, ( uvs_TexCoord21 + appendResult25 ).xy ) * ( 1.0 - temp_output_39_0 ) ) + temp_output_39_0 ) ).rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xyzw = customInputData.uv_texcoord;
				o.customPack1.xyzw = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.VertexColorNode;17;-725.8195,-168.0777;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-790.1162,101.1734;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;202ad81ffa943294294f2b837d8cc3f5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-927.805,279.7126;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StepOpNode;11;-265.343,413.8779;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-119.8449,371.4305;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-479.7338,458.7768;Inherit;False;Property;_Stepper;Stepper;0;0;Create;True;0;0;0;False;0;False;0;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1318.508,152.1734;Inherit;True;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,2;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1280.322,530.7834;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;25;-1085.025,554.2924;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NegateNode;26;-1413.706,643.4171;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1614.262,628.7221;Inherit;False;Property;_FlowSpeed;FlowSpeed;3;0;Create;True;0;0;0;False;0;False;1;10.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;32;-580.8518,599.2053;Inherit;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;0;0;False;0;False;-1;None;1e9de73417d47304fa8dc785110b64cb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-807.2242,839.8861;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-1132.182,908.5448;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;37;-1358.431,926.1079;Inherit;False;Property;_MaskTiling;MaskTiling;5;0;Create;True;0;0;0;False;0;False;0,0;3,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;38;-1543.72,168.375;Inherit;False;Property;_MainTexTiling;MainTexTiling;6;0;Create;True;0;0;0;False;0;False;0,0;3,5.9;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StepOpNode;39;-398.04,236.298;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;43;-338.67,-251.4821;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-70.6856,-59.15812;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;47.89432,90.4043;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;183.3196,-16.06189;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-14.36941,-255.9926;Inherit;False;Property;_Emission;Emission;2;0;Create;True;0;0;0;False;0;False;0;3.52;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-687.9409,356.3192;Inherit;False;Property;_HighLightStepper;HighLightStepper;7;0;Create;True;0;0;0;False;0;False;0;0.73;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;463.6578,-85.86917;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;BeamShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;1;24;0
WireConnection;24;0;21;0
WireConnection;24;1;25;0
WireConnection;11;0;12;0
WireConnection;11;1;32;0
WireConnection;18;0;17;4
WireConnection;18;1;11;0
WireConnection;21;0;38;0
WireConnection;22;0;21;3
WireConnection;22;1;26;0
WireConnection;25;1;22;0
WireConnection;26;0;23;0
WireConnection;32;1;36;0
WireConnection;36;0;35;0
WireConnection;36;1;25;0
WireConnection;35;0;37;0
WireConnection;39;0;40;0
WireConnection;39;1;32;0
WireConnection;43;0;39;0
WireConnection;15;0;17;0
WireConnection;15;1;13;0
WireConnection;15;2;43;0
WireConnection;41;0;15;0
WireConnection;41;1;39;0
WireConnection;44;0;19;0
WireConnection;44;1;41;0
WireConnection;0;2;44;0
WireConnection;0;9;18;0
ASEEND*/
//CHKSM=6904A3C1F4F879E62311A41C1D0ECDC6D425F6E4
// // Made with Amplify Shader Editor
// // Available at the Unity Asset Store - http://u3d.as/y3X 
// Shader "QFX/IFX/Fresnel"
// {
// 	Properties
// 	{
// 		_FresnelScale("Fresnel Scale", Range( 0 , 1)) = 0.510905
// 		_FresnelPower("Fresnel Power", Range( 0 , 5)) = 2
// 		[HDR]_TintColor("Tint Color", Color) = (0,0,0,0)
// 	}
	
// 	SubShader
// 	{
// 		Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
// 		LOD 100
// 		CGINCLUDE
// 		#pragma target 3.0
// 		ENDCG
// 		Blend SrcAlpha OneMinusSrcAlpha
// 		Cull Back
// 		ColorMask RGBA
// 		ZWrite On
// 		ZTest LEqual
		
		

// 		Pass
// 		{
// 			Name "Unlit"
// 			CGPROGRAM
// 			#pragma vertex vert
// 			#pragma fragment frag
// 			#pragma multi_compile_instancing
// 			#include "UnityCG.cginc"
			

// 			struct appdata
// 			{
// 				float4 vertex : POSITION;
// 				UNITY_VERTEX_INPUT_INSTANCE_ID
// 				float3 ase_normal : NORMAL;
// 			};
			
// 			struct v2f
// 			{
// 				float4 vertex : SV_POSITION;
// 				float4 ase_texcoord : TEXCOORD0;
// 				float4 ase_texcoord1 : TEXCOORD1;
// 				UNITY_VERTEX_OUTPUT_STEREO
// 				UNITY_VERTEX_INPUT_INSTANCE_ID
// 			};

// 			uniform float4 _TintColor;
// 			uniform float _FresnelScale;
// 			uniform float _FresnelPower;
			
// 			v2f vert ( appdata v )
// 			{
// 				v2f o;
// 				UNITY_SETUP_INSTANCE_ID(v);
// 				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
// 				UNITY_TRANSFER_INSTANCE_ID(v, o);

// 				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
// 				o.ase_texcoord.xyz = ase_worldPos;
// 				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
// 				o.ase_texcoord1.xyz = ase_worldNormal;
				
				
// 				//setting value to unused interpolator channels and avoid initialization warnings
// 				o.ase_texcoord.w = 0;
// 				o.ase_texcoord1.w = 0;
				
// 				v.vertex.xyz +=  float3(0,0,0) ;
// 				o.vertex = UnityObjectToClipPos(v.vertex);
// 				return o;
// 			}
			
// 			fixed4 frag (v2f i ) : SV_Target
// 			{
// 				UNITY_SETUP_INSTANCE_ID(i);
// 				fixed4 finalColor;
// 				float3 ase_worldPos = i.ase_texcoord.xyz;
// 				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
// 				ase_worldViewDir = normalize(ase_worldViewDir);
// 				float3 ase_worldNormal = i.ase_texcoord1.xyz;
// 				float fresnelNdotV1_g15 = dot( ase_worldNormal, ase_worldViewDir );
// 				float fresnelNode1_g15 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV1_g15, _FresnelPower ) );
// 				float4 appendResult44 = (float4(_TintColor.rgb , ( _TintColor.a * saturate( fresnelNode1_g15 ) )));
				
				
// 				finalColor = appendResult44;
// 				return finalColor;
// 			}
// 			ENDCG
// 		}
// 	}
// 	CustomEditor "ASEMaterialInspector"
	
	
// }
// /*ASEBEGIN
// Version=16100
// 241;244;1084;799;715.8285;409.3423;1.461561;True;False
// Node;AmplifyShaderEditor.FunctionNode;40;-331.9319,262.9335;Float;False;QFX Get Fresnel;0;;15;0a832704e6daa5244b3db55d16dfb317;0;0;1;FLOAT;0
// Node;AmplifyShaderEditor.ColorNode;51;-348.9765,32.0491;Float;False;Property;_TintColor;Tint Color;3;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1.029412,1.549696,5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
// Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-12.06982,122.3265;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
// Node;AmplifyShaderEditor.DynamicAppendNode;44;185.6204,34.29636;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
// Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;35;360.7844,35.61035;Float;False;True;2;Float;ASEMaterialInspector;0;1;QFX/IFX/Fresnel;0770190933193b94aaa3065e307002fa;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;0;False;-1;True;False;0;False;-1;0;False;-1;True;2;RenderType=Transparent=RenderType;Queue=Transparent+1;True;2;0;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
// WireConnection;45;0;51;4
// WireConnection;45;1;40;0
// WireConnection;44;0;51;0
// WireConnection;44;3;45;0
// WireConnection;35;0;44;0
// ASEEND*/
// //CHKSM=8292146C6BBF1E4F1A2751A94E2E376A496473CD


Shader "QFX/IFX/Fresnel_URP"
{
    Properties
    {
        _FresnelScale("Fresnel Scale", Range(0, 1)) = 0.510905
        _FresnelPower("Fresnel Power", Range(0, 5)) = 2
        [HDR]_TintColor("Tint Color", Color) = (0,0,0,0)
        
        // URP specific properties
        [HideInInspector] _Surface("__surface", Float) = 1
        [HideInInspector] _Blend("__blend", Float) = 0
        [HideInInspector] _AlphaClip("__clip", Float) = 0
        [HideInInspector] _SrcBlend("__src", Float) = 5
        [HideInInspector] _DstBlend("__dst", Float) = 10
        [HideInInspector] _ZWrite("__zw", Float) = 1
        [HideInInspector] _Cull("__cull", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+1"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "ShaderModel"="4.5"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite On
        ZTest LEqual
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        ENDHLSL

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                half fogCoord       : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _TintColor;
            float _FresnelScale;
            float _FresnelPower;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Calculate view direction
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - input.positionWS);
                
                // Calculate Fresnel effect
                float NdotV = saturate(dot(input.normalWS, viewDir));
                float fresnel = _FresnelScale * pow(1.0 - NdotV, _FresnelPower);
                fresnel = saturate(fresnel);
                
                // Combine color with Fresnel alpha
                half4 finalColor = _TintColor;
                finalColor.a *= fresnel;
                
                // Apply fog
                finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);
                
                return finalColor;
            }
            ENDHLSL
        }
        
        // Depth only pass for depth texture
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ColorMask 0
            ZWrite On
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            struct DepthAttributes
            {
                float4 positionOS   : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct DepthVaryings
            {
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            DepthVaryings DepthOnlyVertex(DepthAttributes input)
            {
                DepthVaryings output = (DepthVaryings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                
                return output;
            }

            half4 DepthOnlyFragment(DepthVaryings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                return 0;
            }
            ENDHLSL
        }
    }
    
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.CustomFresnelShaderGUI"
    Fallback "Universal Render Pipeline/Unlit"
}
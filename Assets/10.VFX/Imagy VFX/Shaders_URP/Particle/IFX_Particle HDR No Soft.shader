// Shader "QFX/IFX/Particle/Particle HDR No Soft" {
// 	Properties{
// 		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
// 		_MainTex("Particle Texture", 2D) = "white" {}
// 		_POW("Texture POW scale", Float) = 1.0
// 	}

// 		Category{
// 			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
// 			Blend SrcAlpha OneMinusSrcAlpha
// 			Cull Back
// 			ColorMask RGB
// 			Cull Off Lighting Off ZWrite Off

// 			SubShader {
// 				Pass {

// 					CGPROGRAM
// 					#pragma vertex vert
// 					#pragma fragment frag
// 					#pragma multi_compile_particles
// 					#pragma multi_compile_fog

// 					#include "UnityCG.cginc"

// 					sampler2D _MainTex;
// 					fixed4 _TintColor;
// 					float _POW;

// 					struct appdata_t {
// 						float4 vertex : POSITION;
// 						fixed4 color : COLOR;
// 						float2 texcoord : TEXCOORD0;
// 					};

// 					struct v2f {
// 						float4 vertex : SV_POSITION;
// 						fixed4 color : COLOR;
// 						float2 texcoord : TEXCOORD0;
// 						UNITY_FOG_COORDS(1)
// 					};

// 					float4 _MainTex_ST;

// 					v2f vert(appdata_t v)
// 					{
// 						v2f o;
// 						o.vertex = UnityObjectToClipPos(v.vertex);
// 						o.color = v.color;
// 						o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
// 						UNITY_TRANSFER_FOG(o,o.vertex);
// 						return o;
// 					}

// 					sampler2D_float _CameraDepthTexture;

// 					float4 frag(v2f i) : SV_Target
// 					{
// 						float4 tex = tex2D(_MainTex, i.texcoord);
// 						tex = pow(tex,_POW);
// 						float4 col = 2.0f * i.color * _TintColor * tex;
// 						UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
// 						col.a = saturate(col.a);
// 						return col;
// 					}
// 					ENDCG
// 				}
// 			}
// 		}
// }


Shader "QFX/URP/Particle HDR No Soft"
{
    Properties
    {
        [HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex("Particle Texture", 2D) = "white" {}
        _POW("Texture POW scale", Float) = 1.0
    }

    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True" 
            "DisableBatching"="True" 
        }
        LOD 100

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Add fog support for URP
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                // Use the proper fog factor for URP
                half fogFactor : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _TintColor;
                float _POW;
                float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.color = IN.color;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                
                // Calculate fog factor for URP
                OUT.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                tex = pow(tex, _POW);
                half4 col = 2.0 * tex * IN.color * _TintColor;
                
                // Apply fog in URP
                col.rgb = MixFog(col.rgb, IN.fogFactor);
                
                col.a = saturate(col.a);
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
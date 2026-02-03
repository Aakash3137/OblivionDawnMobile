Shader "Toon/TFF_ToonWater_URP"
{
    Properties
    {
        // Water Colors
        _ShallowColor("Shallow Color", Color) = (0,0.6117647,1,1)
        _DeepColor("Deep Color", Color) = (0,0.3333333,0.8509804,1)
        _ShallowColorDepth("Shallow Color Depth", Range(0, 15)) = 2.75
        
        // Transparency
        _Opacity("Opacity", Range(0, 1)) = 0.5
        _OpacityDepth("Opacity Depth", Range(0, 20)) = 6.5
        
        // Foam
        _FoamColor("Foam Color", Color) = (0.8705882,0.8705882,0.8705882,1)
        _FoamHardness("Foam Hardness", Range(0, 1)) = 0.33
        _FoamDistance("Foam Distance", Range(0, 1)) = 0.05
        _FoamOpacity("Foam Opacity", Range(0, 1)) = 0.65
        _FoamScale("Foam Scale", Range(0, 1)) = 0.2
        _FoamSpeed("Foam Speed", Range(0, 1)) = 0.125
        
        // Fresnel
        _FresnelColor("Fresnel Color", Color) = (0.8313726,0.8313726,0.8313726,1)
        _FresnelIntensity("Fresnel Intensity", Range(0, 1)) = 0.4
        
        // Waves
        [Toggle(_WAVES_ON)] _Waves("Waves", Float) = 1
        _WaveAmplitude("Wave Amplitude", Range(0, 1)) = 0.5
        _WaveIntensity("Wave Intensity", Range(0, 1)) = 0.15
        _WaveSpeed("Wave Speed", Range(0, 1)) = 1
        
        // Reflections
        _ReflectionsOpacity("Reflections Opacity", Range(0, 1)) = 0.65
        _ReflectionsScale("Reflections Scale", Range(1, 40)) = 4.8
        _ReflectionsScrollSpeed("Reflections Scroll Speed", Range(-1, 1)) = 0.05
        _ReflectionsCutoff("Reflections Cutoff", Range(0, 1)) = 0.35
        _ReflectionsCutoffScale("Reflections Cutoff Scale", Range(1, 40)) = 3
        _ReflectionsCutoffScrollSpeed("Reflections Cutoff Scroll Speed", Range(-1, 1)) = -0.025
        
        // Textures
        [Normal]_NormalMap("Normal Map", 2D) = "bump" {}
        _NoiseTexture("Noise Texture", 2D) = "white" {}
        
        // URP Specific
        _Surface("__surface", Float) = 0.0
        _Blend("__blend", Float) = 0.0
        _AlphaClip("__clip", Float) = 0.0
        _SrcBlend("__src", Float) = 1.0
        _DstBlend("__dst", Float) = 0.0
        _ZWrite("__zw", Float) = 1.0
        _Cull("__cull", Float) = 2.0
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
    }
    
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "ForceNoShadowCasting"="True"
        }
        
        LOD 300
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            // URP Keywords
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
            
            // Shader Features
            #pragma shader_feature _WAVES_ON
            
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float3 viewDirWS : TEXCOORD6;
                float fogCoord : TEXCOORD7;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // Properties
            CBUFFER_START(UnityPerMaterial)
                // Colors
                half4 _ShallowColor;
                half4 _DeepColor;
                half _ShallowColorDepth;
                
                // Transparency
                half _Opacity;
                half _OpacityDepth;
                
                // Foam
                half4 _FoamColor;
                half _FoamHardness;
                half _FoamDistance;
                half _FoamOpacity;
                half _FoamScale;
                half _FoamSpeed;
                
                // Fresnel
                half4 _FresnelColor;
                half _FresnelIntensity;
                
                // Waves
                half _WaveAmplitude;
                half _WaveIntensity;
                half _WaveSpeed;
                
                // Reflections
                half _ReflectionsOpacity;
                half _ReflectionsScale;
                half _ReflectionsScrollSpeed;
                half _ReflectionsCutoff;
                half _ReflectionsCutoffScale;
                half _ReflectionsCutoffScrollSpeed;
            CBUFFER_END
            
            // Textures
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;
            
            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);
            
            // Helper Functions
            half Remap(half value, half inMin, half inMax, half outMin, half outMax)
            {
                return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
            }
            
            half3 UnpackNormalURP(half4 packedNormal)
            {
                half3 normal;
                normal.xy = packedNormal.rg * 2.0 - 1.0;
                normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
                return normal;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                // Calculate world position
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                // Wave displacement
                #ifdef _WAVES_ON
                float2 uv_NormalMap = TRANSFORM_TEX(input.uv, _NormalMap);
                half3 normalMap = UnpackNormalURP(SAMPLE_TEXTURE2D_LOD(_NormalMap, sampler_NormalMap, uv_NormalMap, 0));
                
                half waveTime = _Time.y * _WaveSpeed;
                half waveDisplacement = sin(waveTime - (normalMap.b * (_WaveAmplitude * 30.0)));
                half waveIntensity = Remap(_WaveIntensity, 0.0, 1.0, 0.0, 0.15);
                
                input.positionOS.xyz += input.normalOS * (waveDisplacement * waveIntensity);
                #endif
                
                // Recalculate with displacement
                vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, 1.0);
                output.bitangentWS = normalInput.bitangentWS;
                output.uv = input.uv;
                
                // Screen position for depth calculations
                output.screenPos = ComputeScreenPos(output.positionCS);
                
                // View direction
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(output.positionWS);
                
                // Fog
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Screen space calculations
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float rawDepth = SampleSceneDepth(screenUV);
                float sceneDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float surfaceDepth = input.screenPos.w;
                float depthDifference = sceneDepth - surfaceDepth;
                
                // Depth-based colors
                float depth01 = saturate(depthDifference / _ShallowColorDepth);
                half4 waterColor = lerp(_ShallowColor, _DeepColor, depth01);
                
                // Fresnel effect
                float fresnel = 1.0 - saturate(dot(input.normalWS, input.viewDirWS));
                float fresnelPower = Remap(_FresnelIntensity, 1.0, 0.0, 0.0, 10.0);
                float fresnelEffect = pow(fresnel, fresnelPower);
                fresnelEffect = saturate(fresnelEffect);
                half4 colorWithFresnel = lerp(waterColor, _FresnelColor, fresnelEffect);
                
                // Normal mapping
                float2 normalUV = TRANSFORM_TEX(input.uv, _NormalMap);
                half3 normalTS = UnpackNormalURP(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, normalUV));
                
                // Transform normal to world space
                float3x3 tangentToWorld = float3x3(
                    input.tangentWS.xyz,
                    input.bitangentWS,
                    input.normalWS
                );
                half3 normalWS = mul(normalTS, tangentToWorld);
                
                // Foam calculations
                // First foam layer
                float foamTime = _Time.y * Remap(_FoamSpeed, 0.0, 1.0, 0.0, 2.5);
                float foamScale = Remap(_FoamScale, 0.0, 1.0, 30.0, 1.0);
                float2 foamUV1 = foamTime + (input.uv * foamScale);
                half noise1 = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, foamUV1).r;
                
                float foamDistance1 = Remap(_FoamDistance, 0.0, 1.0, 0.0, 10.0);
                float foamDepth1 = saturate(depthDifference / (noise1 * foamDistance1));
                float foamHardness = Remap(_FoamHardness, 0.0, 1.0, 1.0, 10.0);
                float foamMask1 = pow(foamDepth1, foamHardness);
                foamMask1 = saturate(foamMask1);
                float foam1 = (1.0 - foamMask1) * _FoamOpacity;
                
                // Second foam layer
                float2 foamUV2 = (_Time.y * _FoamSpeed) + (input.uv * Remap(_FoamScale, 0.0, 1.0, 15.0, 1.0));
                half noise2 = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, foamUV2).r;
                float foamDistance2 = Remap(_FoamDistance, 0.0, 1.0, 0.0, 15.0);
                float foamDepth2 = saturate(depthDifference / foamDistance2);
                float foamMask2 = 1.0 - foamDepth2;
                float foamOpacity2 = Remap(_FoamOpacity, 0.0, 1.0, 0.0, 0.85);
                float foam2 = foamMask2 * (noise2 * foamOpacity2);
                
                // Combine foam
                half4 foamColor1 = _FoamColor * foam1;
                half4 foamColor2 = _FoamColor * foam2;
                
                // Reflections
                float2 reflectionsUV = (_Time.y * _ReflectionsScrollSpeed) + (input.uv * _ReflectionsScale);
                half3 reflectionsNormal = UnpackNormalURP(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, reflectionsUV));
                float reflectionsStrength = reflectionsNormal.g * Remap(_ReflectionsOpacity, 0.0, 1.0, 0.0, 8.0);
                
                // Get main light
                Light mainLight = GetMainLight();
                half3 lightDir = mainLight.direction;
                half3 lightColor = mainLight.color;
                half lightAtten = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
                
                // Reflection cutoff
                float2 cutoffUV = (_Time.y * _ReflectionsCutoffScrollSpeed) + 
                                 (input.uv * Remap(_ReflectionsCutoffScale, 0.0, 10.0, 2.0, 10.0));
                half3 cutoffNormal = UnpackNormalURP(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, cutoffUV));
                half3 cutoffNormalWS = mul(cutoffNormal, tangentToWorld);
                
                // Calculate reflection vector
                half3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
                half3 reflectDir = reflect(-viewDir, cutoffNormalWS);
                
                // Reflection intensity
                half reflectionDot = dot(reflectDir, lightDir);
                half reflectionCutoff = Remap(_ReflectionsCutoff, 0.0, 1.0, 0.0, 10.0);
                half reflectionPower = pow(reflectionDot, exp(reflectionCutoff));
                
                half3 reflections = (reflectionPower * lightColor) * reflectionsStrength;
                reflections = saturate(reflections);
                
                // Combine reflections with lighting
                half3 reflectedLight = reflections * float3(input.uv, 0.0);
                half3 directLight = lightColor * lightAtten;
                half3 finalLighting = lerp(reflectedLight, directLight, 1.0 - lightAtten);
                
                // Final color composition
                half4 baseColor = colorWithFresnel;
                baseColor.rgb += foamColor1.rgb + foamColor2.rgb;
                
                // Apply lighting
                half3 litColor = baseColor.rgb * lerp(half3(1,1,1), lightColor, 0.75);
                litColor += reflections;
                
                // Calculate opacity
                float opacityDepth = saturate(depthDifference / _OpacityDepth);
                float finalOpacity = saturate((foam1 + foam2) + _Opacity + opacityDepth);
                
                // Apply fog
                litColor = MixFog(litColor, input.fogCoord);
                
                return half4(litColor, finalOpacity);
            }
            ENDHLSL
        }
        
        // Shadow casting pass (simplified - water usually doesn't cast shadows)
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            #pragma shader_feature _WAVES_ON
            
            struct ShadowAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct ShadowVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            CBUFFER_START(UnityPerMaterial)
                half _WaveAmplitude;
                half _WaveIntensity;
                half _WaveSpeed;
            CBUFFER_END
            
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;
            
            ShadowVaryings vertShadow(ShadowAttributes input)
            {
                ShadowVaryings output;
                UNITY_SETUP_INSTANCE_ID(input);
                
                #ifdef _WAVES_ON
                float2 uv_NormalMap = TRANSFORM_TEX(input.uv, _NormalMap);
                half3 normalMap = UnpackNormalURP(SAMPLE_TEXTURE2D_LOD(_NormalMap, sampler_NormalMap, uv_NormalMap, 0));
                
                half waveTime = _Time.y * _WaveSpeed;
                half waveDisplacement = sin(waveTime - (normalMap.b * (_WaveAmplitude * 30.0)));
                half waveIntensity = Remap(_WaveIntensity, 0.0, 1.0, 0.0, 0.15);
                
                input.positionOS.xyz += input.normalOS * (waveDisplacement * waveIntensity);
                #endif
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            half4 fragShadow(ShadowVaryings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
        
        // Depth only pass for depth prepass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vertDepth
            #pragma fragment fragDepth
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #pragma shader_feature _WAVES_ON
            
            struct DepthAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct DepthVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            CBUFFER_START(UnityPerMaterial)
                half _WaveAmplitude;
                half _WaveIntensity;
                half _WaveSpeed;
            CBUFFER_END
            
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;
            
            DepthVaryings vertDepth(DepthAttributes input)
            {
                DepthVaryings output;
                UNITY_SETUP_INSTANCE_ID(input);
                
                #ifdef _WAVES_ON
                float2 uv_NormalMap = TRANSFORM_TEX(input.uv, _NormalMap);
                half3 normalMap = UnpackNormalURP(SAMPLE_TEXTURE2D_LOD(_NormalMap, sampler_NormalMap, uv_NormalMap, 0));
                
                half waveTime = _Time.y * _WaveSpeed;
                half waveDisplacement = sin(waveTime - (normalMap.b * (_WaveAmplitude * 30.0)));
                half waveIntensity = Remap(_WaveIntensity, 0.0, 1.0, 0.0, 0.15);
                
                input.positionOS.xyz += input.normalOS * (waveDisplacement * waveIntensity);
                #endif
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            half4 fragDepth(DepthVaryings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
    
    Fallback "Universal Render Pipeline/Unlit"
}
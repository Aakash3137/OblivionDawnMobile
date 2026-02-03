Shader "QFX/IFX/Cutout/AlphaBlendNoiseCutout_URP"
{
    Properties
    {
        [HDR]_TintColor("Tint Color", Color) = (0,0,0,0)
        _NoiseTex("Noise Tex", 2D) = "white" {}
        _Cutoff("Mask Clip Value", Float) = 0.5
        [HDR]_DissolveColor("Dissolve Color", Color) = (1,1,1,1)
        _AlphaCutout("Alpha Cutout", Float) = 0
        _MainTex("Main Tex", 2D) = "white" {}
        _TexPower("Tex Power", Float) = 1
        _DissolveEdgeWidth("Dissolve Edge Width", Range(0,1)) = 0
        _NoiseSpeed("Noise Speed", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="TransparentCutout"
            "IgnoreProjector"="True"
            "IsEmissive"="true"
        }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode"="UniversalForward" }

            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;     // MainTex UV
                float2 uv2 : TEXCOORD1;    // Noise UV
                float3 uv3 : TEXCOORD2;    // uv3.z is used!
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 uv3 : TEXCOORD2;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex);  SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _NoiseTex_ST;

                float4 _TintColor;
                float4 _DissolveColor;

                float2 _NoiseSpeed;

                float _TexPower;
                float _AlphaCutout;
                float _DissolveEdgeWidth;
                float _Cutoff;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.uv2 = v.uv2;
                o.uv3 = v.uv3;
                o.color = v.color;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float time = _Time.y;

                // -------- Noise / Opacity mask --------
                float2 noiseUV = TRANSFORM_TEX(i.uv2, _NoiseTex);
                noiseUV += time * _NoiseSpeed;

                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;

                float opacityMask =
                    (1.0 - (_AlphaCutout + i.uv3.z)) - noise;

                float opacityMaskSat = saturate(opacityMask);

                // -------- Main texture / emission --------
                float2 mainUV = TRANSFORM_TEX(i.uv, _MainTex);
                float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUV);

                float mainR = pow(mainTex.r, _TexPower);

                float4 emission = mainR * _TintColor * i.color;

                float alpha = emission.a * i.color.a;

                // -------- Dissolve edge --------
                float3 finalColor =
                    (_DissolveEdgeWidth > opacityMaskSat)
                    ? _DissolveColor.rgb
                    : emission.rgb;

                // -------- Clip --------
                clip((emission.a * opacityMask) - _Cutoff);

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
Shader "QFX/IFX/Decal/URP/DepthMask"
{
    Properties
    {
        // No properties needed
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "DepthMask"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off
            ZWrite On
            ColorMask 0 // Do not write to color buffer

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                // No color output; just depth
                return float4(0,0,0,0);
            }

            ENDHLSL
        }
    }
}

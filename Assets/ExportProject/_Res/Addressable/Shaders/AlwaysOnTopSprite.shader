Shader "Custom/AlwaysOnTopSpriteWithColorControl"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1) // 添加颜色控制属性
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent+1"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            ZWrite Off
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor; // 引入颜色控制变量

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _TintColor; // 应用颜色控制
                return col;
            }
            ENDCG
        }
    }
}
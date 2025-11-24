// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Sprite/SpriteFill"
{Properties
    {
        [KeywordEnum(VERTICAL_ON, HORIZONTAL_ON, CIRCLE_ON)] _Mode ("Fill Mode", Float) = 1
        [Toggle(_Direction_ON)] _FillDir("Positive Fill",Float) = 1
        _MainColor("Color",Color) = (1,0,0,1)
        _Text ("Text",2D) = "white"
        [PerRendererData] _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Rate("rate",Range(0.0,1.0)) = 0
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing 
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma shader_feature  _MODE_VERTICAL_ON  _MODE_HORIZONTAL_ON  _MODE_CIRCLE_ON  
            #pragma shader_feature _Direction_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _Text;
             float4 _MainTex_ST;
            fixed4 _MainColor;
            float _Rate;
            float cli;
            float _StartPos;
            
            v2f vert(appdata_t v)
            {
                v2f o;
                //UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                fixed4 colo = tex2D(_Text,i.texcoord);               
                //垂直填充
                #if _MODE_VERTICAL_ON             
                    //下侧开始
                    #if _Direction_ON
                        clip(_Rate - i.texcoord.y);
                    //上侧开始
                    #else              
                        clip(i.texcoord.y - (1-_Rate));
                    #endif
                #endif
                //水平填充
                #if _MODE_HORIZONTAL_ON
                    //右侧开始
                    #if _Direction_ON                
                         clip(i.texcoord.x - (1-_Rate));    
                    //左侧开始            
                    #else              
                         clip(_Rate - i.texcoord.x);
                    #endif
                #endif
                //原型填充
                #if _MODE_CIRCLE_ON                 
                    half2 cuv = i.texcoord - half2(0.5, 0.5);;
                    half2 luv = half2(1, 0);
                    half2 s = cuv.x * luv.y - luv.x * cuv.y;
                    half2 c = cuv.x * luv.x + cuv.y * luv.y;
                    half2 angle = 0;
                    //逆时针
                    #if _Direction_ON
                         angle = atan2(s, c) * (180 / 3.1416);              
                    //顺时针
                    #else                
                         angle = atan2(s, -c) * (180 / 3.1416);
                    #endif             
                    angle += step(0, cuv.y) * 360;
                    clip(_Rate * 360 - angle);
                #endif
                return col * _MainColor * colo;
            }
            ENDCG
        }
    }

}

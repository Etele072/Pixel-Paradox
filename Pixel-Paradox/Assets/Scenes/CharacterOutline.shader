Shader "Custom/CharacterOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 5)) = 1
        _OutlineEnabled ("Outline Enabled", Float) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineEnabled;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                if (_OutlineEnabled < 0.5)
                {
                    col.rgb *= col.a;
                    return col;
                }

                float2 size = _MainTex_TexelSize.xy * _OutlineWidth;
                
                float left  = tex2D(_MainTex, IN.texcoord + float2(-size.x, 0)).a;
                float right = tex2D(_MainTex, IN.texcoord + float2(size.x, 0)).a;
                float up    = tex2D(_MainTex, IN.texcoord + float2(0, size.y)).a;
                float down  = tex2D(_MainTex, IN.texcoord + float2(0, -size.y)).a;

                if (col.a > 0.1 && (left < 0.9 || right < 0.9 || up < 0.9 || down < 0.9))
                {
                    fixed4 o = _OutlineColor;
                    o.rgb *= o.a;
                    return o;
                }

                col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
}
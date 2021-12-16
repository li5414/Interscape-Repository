
Shader "Unlit/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _Color2 ("Secondary Color", Color) = (1,1,1,1)
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1  //2.75 is good
        _TileColours ("Tile Colors", 2D) = "white" {}
        _Depth ("Depth", Float) = 1
        _Cutoff ("Cutoff", Float) = 0
        _Opacity ("Opacity", Float) = 0
        _Amplitude ("Amplitude", Float) = 0.1
    }

    SubShader
    {
        Tags{"Queue" = "Transparent" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
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
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            // color from the material
            fixed4 _Color;
            fixed4 _Color2;
            fixed4 _FoamColor;
            float _Depth;
            float _Cutoff;
            float _Opacity;
            float _Amplitude;
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _NoiseScale;
            sampler2D _TileColours;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = v.texcoord;

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);
                return color;
            }

            
            // pixel shader
            fixed4 frag (v2f IN) : SV_Target
            //void surf (v2f IN)
            {
                //get biome color
                float2 pos;
                pos.x = (((abs (IN.worldPosition.x))) % 5000)/5000;
                pos.y = (((abs (IN.worldPosition.y))) % 5000)/5000;
                fixed4 col = tex2D (_TileColours, pos);

                //get shape
                //fixed4 c = _Color;
                fixed4 c = lerp(_Color, _Color2, col.a);
                float opacity = _Opacity + (_Amplitude * sin(_Time.y));

                c.a = (_Color.a - (_Depth * col.a)) + opacity;

                float rememberOpacity = c.a;
                //float startDepth = 0.7;
                //float currentDepth = startDepth + (0.1 * tan(-_Time.y * 0.5));
                float width = 0.005;

                /*if (c.a > currentDepth - width && c.a < currentDepth + width) {
                    c = _FoamColor;
                }*/

                if (c.a < _Cutoff + (width * 2)) {
                    c = _FoamColor;
                }

                if (rememberOpacity < _Cutoff) {
                    c.a = 0;
                }

                
                return c;
            }
            ENDCG
        }
    }
}

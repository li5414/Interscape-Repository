
Shader "Unlit/ChunkWaterShader" {
    Properties {
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

    SubShader {
        Tags{"Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
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

            v2f vert(appdata_t v) {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            // pixel shader
            fixed4 frag (v2f IN) : SV_Target {
                // get biome color from texture
                float2 uv = (IN.texcoord - 0.5) * 0.8888888 + 0.5; // Note: 0.88888 is 16 (the chunk size) divided by 18 (the size of texture)
                fixed4 col = tex2D (_TileColours, uv);

                // use alpha value of texture to determine depth of water
                fixed4 c = lerp(_Color, _Color2, col.a);
                float opacity = _Opacity - (_Amplitude * (sin(_Time.y) + 1));

                c.a = (_Color.a - (_Depth * col.a)) + opacity;

                float rememberOpacity = c.a;
                float width = 0.005;

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

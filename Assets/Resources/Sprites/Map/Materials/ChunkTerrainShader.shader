
Shader "Unlit/ChunkTerrainShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1
        _TileColours ("Tile Colors", 2D) = "white" {}
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
            {
                //get biome color
                float2 pos;
                
                // scale texture up
                float2 uv = (IN.texcoord - 0.5) * 0.93 + 0.5; // Note: 0.88888 is 16 (the chunk size) divided by 18 (the size of texture)
                
                // get biome color from texture
                fixed4 col = tex2D (_TileColours, uv);
                // fixed4 col = tex2D (_TileColours, IN.texcoord);

                col.a = 1;

                //get shape
                fixed4 c = SampleSpriteTexture (IN.texcoord) * col;
               
                // get some noisee
                pos.x = (((abs (IN.worldPosition.x)) * _NoiseScale) % 256)/256;
                pos.y = (((abs (IN.worldPosition.y)) * _NoiseScale) % 256)/256;
                float noise = tex2D (_NoiseTex, pos);

                // use noise to help blend colours a bit more
                c.b = (c.b + (0.06 * noise));
                c.r = (c.r + (0.10 * noise));
                c.g = (c.g + (0.12 * noise));
                c.rgb -= 0.03;
                return c;
            }
            ENDCG
        }
    }
}

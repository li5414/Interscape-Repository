
Shader "Unlit/GrassBlade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1  //2.75 is good
        _TileColours ("Tile Colors", 2D) = "white" {}
    }

    SubShader
    {
        Tags{"Queue" = "Transparent" "RenderType"="Transparent" }

        //Blend SrcAlpha OneMinusSrcAlpha // not sure if need

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

            
            // pixel shader, no inputs needed
            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                float2 pos;
                pos.x = (((abs (IN.worldPosition.x)) * _NoiseScale) % 256)/256;
                pos.y = (((abs (IN.worldPosition.y)) * _NoiseScale) % 256)/256;
                float noise = tex2D (_NoiseTex, pos);
                return noise * c;
            }
            ENDCG
        }
    }
}

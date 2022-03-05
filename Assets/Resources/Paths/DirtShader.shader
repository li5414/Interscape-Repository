Shader "Unlit/DirtShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1
        _SpeckleTex ("Speckle Texture", 2D) = "white" {}
        _SpeckleScale ("Speckle Scale", Float) = 1
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
            sampler2D _SpeckleTex;
            float _SpeckleScale;

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
                // get shape
                fixed4 c = SampleSpriteTexture (IN.texcoord) * _Color;
               
                // get some noisee
                float2 pos;
                pos.x = ((abs (IN.worldPosition.x) * _NoiseScale) % 512)/512;
                pos.y = ((abs (IN.worldPosition.y) * _NoiseScale) % 512)/512;
                fixed4 noise = tex2D (_NoiseTex, pos);

                float2 pos2;
                pos2.x = ((abs (IN.worldPosition.x) * _SpeckleScale) % 256)/256;
                pos2.y = ((abs (IN.worldPosition.y) * _SpeckleScale) % 256)/256;
                float speckle = tex2D (_SpeckleTex, pos2).a * 0.1;

                c.g -= (1 - noise.g) / 2 + speckle;
                c.b -= (1 - noise.b) / 2 + speckle;
                c.r -= (1 - noise.r) / 3 + speckle;

                 
                return c;
            }
            ENDCG
        }
    }
}

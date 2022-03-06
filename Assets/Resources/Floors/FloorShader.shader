Shader "Unlit/FloorShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _DetailTex ("Detail Texture", 2D) = "white" {}
        _Scale ("Scale", Float) = 1
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
            sampler2D _DetailTex;
            float _Scale;

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
                pos.x = ((abs (IN.worldPosition.x) * _Scale) % 512)/512;
                pos.y = ((abs (IN.worldPosition.y) * _Scale) % 512)/512;
                fixed4 details = tex2D (_DetailTex, pos);

                return c * details;
            }
            ENDCG
        }
    }
}

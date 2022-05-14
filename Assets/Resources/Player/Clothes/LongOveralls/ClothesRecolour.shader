Shader "Unlit/ClothesRecolour"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SwapTex("Color Data", 2D) = "transparent" {}
    }
    SubShader
    {
        Tags{"Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off  
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
                float4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SwapTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.uv);
                fixed4 swapCol = tex2D(_SwapTex, float2(c.r, 0));
                // fixed4 final = lerp(c, swapCol, swapCol.a) * IN.color;
                // final.a = c.a;
                // final.rgb *= c.a;
                swapCol.a = c.a;
                return swapCol;
            }
            ENDCG
        }
    }
}

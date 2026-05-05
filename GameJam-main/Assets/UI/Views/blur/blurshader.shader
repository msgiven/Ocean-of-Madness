Shader "Custom/ScreenBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 0.02)) = 0.005
        _Iterations ("Iterations", Range(1, 10)) = 5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
            float _BlurSize;
            int _Iterations;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,0);
                float2 blurOffset = _BlurSize / _Iterations;
                
                for(int idx = 0; idx < _Iterations; idx++)
                {
                    float2 offset = float2(blurOffset * (idx - _Iterations/2), 0);
                    col += tex2D(_MainTex, i.uv + offset);
                }
                
                for(int idx = 0; idx < _Iterations; idx++)
                {
                    float2 offset = float2(0, blurOffset * (idx - _Iterations/2));
                    col += tex2D(_MainTex, i.uv + offset);
                }
                
                col /= (_Iterations * 2);
                return col;
            }
            ENDCG
        }
    }
}
Shader "Custom/NoiseWaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _WaveStrength ("Wave Strength", Range(0, 1)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0, 3)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
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
            sampler2D _NoiseTex;
            float _WaveStrength;
            float _WaveSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                // Sample the noise texture with time-based animation
                float2 animatedUV = i.uv + _Time.y * _WaveSpeed;
                float2 noiseValue = tex2D(_NoiseTex, animatedUV).xy;

                // Distort the original UVs based on the noise value and wave strength
                float2 distortedUV = i.uv + noiseValue * _WaveStrength;
                
                // Sample the main texture with distorted UVs
                half4 col = tex2D(_MainTex, distortedUV);
                return col;
            }
            ENDCG
        }
    }
}

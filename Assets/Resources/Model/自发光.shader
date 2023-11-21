Shader "Unlit/自发光"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionMask ("Emission Mask", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionMask;
        };

        sampler2D _MainTex;
        sampler2D _EmissionMask;
        float4 _EmissionColor;
        float4 _Color;

        void surf (Input IN, inout SurfaceOutput o)
        {
            float3 mainTex = tex2D(_MainTex, IN.uv_MainTex).rgb;
            float3 emissionMask = tex2D(_EmissionMask, IN.uv_EmissionMask).rgb;
            o.Albedo = mainTex * _Color.rgb;
            o.Emission = emissionMask * _EmissionColor.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

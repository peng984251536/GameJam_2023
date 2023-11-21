Shader "EmissionGrass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionMask ("Emission Mask", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


    struct Attributes
    {
        float4 position : POSITION;
        float2 uv : TEXCOORD0;
        float4 normal : NORMAL;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
        float4 color : TEXCOORD1;
        float3 posWS : TEXCOORD2;
        float3 normalWS : TEXCOORD3;
        UNITY_VERTEX_OUTPUT_STEREO
    };
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }


        Pass
        {
            Name "EmissonGrass"
            Tags
            {
                "LightMode" = "UniversalForward" "Queue"="Opaque" "RenderType"="Opaque"
            }


            
            ZWrite On
            ZTest LEqual
            Cull Back
            //Blend SrcAlpha OneMinusSrcAlpha
            //Blend Zero SrcColor

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            #pragma multi_compile_local _SOURCE_DEPTH _SOURCE_DEPTH_NORMALS _SOURCE_GBUFFER
            #pragma multi_compile_local _RECONSTRUCT_NORMAL_LOW _RECONSTRUCT_NORMAL_MEDIUM _RECONSTRUCT_NORMAL_HIGH
            #pragma multi_compile_local _ _ORTHOGRAPHIC

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_EmissionMask);
            SAMPLER(sampler_EmissionMask);

            float4 _Color;
            float4 _EmissionColor;
            float _Intensity;


            Varyings VertDefault(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input); //GPU实例
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 posOS = input.position;
                output.positionCS = TransformObjectToHClip(posOS);
                output.uv = input.uv;
                output.posWS = TransformObjectToWorld(posOS);
                output.normalWS = TransformObjectToWorldNormal(input.normal);
                return output;
            }


            float4 Frag(Varyings i) : SV_Target
            {
                float3 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;
                float3 emissionMask = SAMPLE_TEXTURE2D_X(_EmissionMask, sampler_EmissionMask, i.uv).rgb;
                float3 Albedo = mainTex * _Color.rgb;
                float3 Emission = emissionMask * _EmissionColor.rgb;

                return float4(Albedo + Emission, 1);
            }
            ENDHLSL
        }
    }
}
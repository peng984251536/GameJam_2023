Shader "musicItemShader"
{

    Properties
    {
        [HDR]_Color("_Color",color) = (1,1,1,1)
        _ColorEmisson("_ColorEmisson",color) = (1,1,1,1)
        _FresnelScale("_FresnelScale",float) = 0.5
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    float3 _MusicItemColor;
    //float3 _ColorEmisson;
    float _FresnelScale;

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
        //        Cull Off 
        //        ZWrite Off 
        //        ZTest LEqual

        Pass
        {
            Name "MyMusicItem"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            ZTest Never
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            #pragma multi_compile_local _SOURCE_DEPTH _SOURCE_DEPTH_NORMALS _SOURCE_GBUFFER
            #pragma multi_compile_local _RECONSTRUCT_NORMAL_LOW _RECONSTRUCT_NORMAL_MEDIUM _RECONSTRUCT_NORMAL_HIGH
            #pragma multi_compile_local _ _ORTHOGRAPHIC


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
                float3 viewDir = -normalize(i.posWS.xyz - _WorldSpaceCameraPos);
                float NoV = dot(viewDir, i.normalWS);
                float fresnel = _FresnelScale + (1 - _FresnelScale) * Pow4(1 - NoV);

                float3 diffuse = saturate(dot(i.normalWS,_MainLightColor));
                
                float3 color = (_MusicItemColor) * (fresnel+diffuse)+ UNITY_LIGHTMODEL_AMBIENT;
                return float4(color+(_MusicItemColor*1.3), 1);
            }
            ENDHLSL
        }
    }
}
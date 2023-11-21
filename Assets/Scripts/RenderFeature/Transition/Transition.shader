Shader "TransitionShader"
{

    Properties
    {
        //_DecalTex("_DecalTex",2D) = "while"{}
        [HDR]_EdgeColor("_EdgeColor",color) = (1,1,1,1)
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
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
            Name "TransitionShader"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            ZTest Always
            ZWrite Off
            Cull Off

            Stencil
            {
                Ref 2
                Comp LEqual
                Pass replace
            }

            //Blend SrcAlpha OneMinusSrcAlpha
            //Blend Zero SrcColor

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            #pragma multi_compile_local _SOURCE_DEPTH _SOURCE_DEPTH_NORMALS _SOURCE_GBUFFER
            #pragma multi_compile_local _RECONSTRUCT_NORMAL_LOW _RECONSTRUCT_NORMAL_MEDIUM _RECONSTRUCT_NORMAL_HIGH
            #pragma multi_compile_local _ _ORTHOGRAPHIC

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D(_PlanarReflectionTexture);
            SAMPLER(sampler_PlanarReflectionTexture);
            TEXTURE2D_X(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);

            float4x4 _VPMatrix_invers;
            float3 _PlayerPos;
            float _Width;
            float _TransitionLength;
            float4 _EdgeColor;
            float4 _RunWithColor;

            float3 GetPosWS(float2 screenUV)
            {
                float depth_o = SAMPLE_TEXTURE2D_X(
                    _CameraDepthTexture, sampler_CameraDepthTexture, screenUV).r;

                //把ndc坐标转换到世界空间
                float4 ndc = float4(screenUV * 2 - 1, (1 - depth_o) * 2 - 1, 1);
                float4 posWS = mul(_VPMatrix_invers, ndc);
                posWS /= posWS.w;

                return posWS.xyz;
            }


            Varyings VertDefault(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input); //GPU实例
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 posOS = input.position;
                output.positionCS = TransformObjectToHClip(posOS);
                output.uv = input.uv;
                return output;
            }


            float4 Frag(Varyings i) : SV_Target
            {
                //return i.color.g;
                float2 screenUV = i.positionCS.xy / (_ScaledScreenParams.xy);
                // float noiseColor = SAMPLE_TEXTURE2D_X(_NoiseTexture,
                //                                       sampler_NoiseTexture, screenUV).r;
                float3 baseColor = SAMPLE_TEXTURE2D(_PlanarReflectionTexture,
                                                    sampler_PlanarReflectionTexture, screenUV).rgb;
                float3 cameraColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture,
                                                      sampler_CameraOpaqueTexture, screenUV).rgb;
                float3 posWS = GetPosWS(screenUV);


                float d = distance(posWS, _PlayerPos);
                // float length1 = step((d)*(noiseColor*0.7+0.7)-_Width, _TransitionLength*2);
                // if(_TransitionLength*2<(d)*(noiseColor*0.7+0.7)+_Width&&
                //     _TransitionLength*2>(d)*(noiseColor*0.7+0.7)-_Width)
                // {
                //     return float4(_EdgeColor.rgb*(baseColor+cameraColor),1);
                // }
                float length1 = step(d-_Width, _TransitionLength*2);
                if(_TransitionLength*2<(d)+_Width&&
                    _TransitionLength*2>(d)-_Width)
                {
                    return float4(_RunWithColor.rgb*6*(cameraColor),1);
                }

                
                //return length1;
                //float length2 = step( (d+_Width)*(noiseColor*0.7+0.7),_TransitionLength*2);

                float3 finalColor = lerp(cameraColor, baseColor,length1);
                //float3 finalColor = lerp(cameraColor, baseColor, length2);
                return float4(finalColor,1);

            }
            ENDHLSL
        }
    }
}
Shader "MistySampleShader"
{
    Properties
    {
        _FogNoiseTex2("_FogNoiseTex2",2D) = "while"{}
        _Intensity("_Intensity",float) = 1

        _MaxInt("_MaxInt",float) = 1
        _MinInt("_MinInt",float) = 0

        _WindDir("_WindDir",vector) = (0,0,0,0)
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
        float3 posWS : TEXCOORD2;
        float3 normalWS : TEXCOORD3;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    //光线步进计算
    inline float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir)
    {
        float3 t0 = (boundsMin - rayOrigin) * invRaydir;
        float3 t1 = (boundsMax - rayOrigin) * invRaydir;
        float3 tmin = min(t0, t1);
        float3 tmax = max(t0, t1);

        float dstA = max(max(tmin.x, tmin.y), tmin.z);
        float dstB = min(tmax.x, min(tmax.y, tmax.z));

        float dstToBox = max(0, dstA);
        float dstInsideBox = max(0, dstB - dstToBox);
        return float2(dstToBox, dstInsideBox);
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }


        Pass
        {
            Name "MistySample"
            Tags
            {
                "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"
            }


            Cull Off
            ZWrite Off
            ZTest Always

            //Blend SrcAlpha OneMinusSrcAlpha
            //Blend Zero SrcColor

            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            #pragma multi_compile_local _SOURCE_DEPTH _SOURCE_DEPTH_NORMALS _SOURCE_GBUFFER
            #pragma multi_compile_local _RECONSTRUCT_NORMAL_LOW _RECONSTRUCT_NORMAL_MEDIUM _RECONSTRUCT_NORMAL_HIGH
            #pragma multi_compile_local _ _ORTHOGRAPHIC

            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D(_CamColorTexture);
            SAMPLER(sampler_CamColorTexture);
            TEXTURE2D_X(_FogNoiseTex2);
            SAMPLER(sampler_FogNoiseTex2);

            float4x4 _VPMatrix_invers;

            float _Intensity;
            float _MaxInt;
            float _MinInt;

            float4 _WindDir;
            float3 _PlayerPos;
            float _OutRadius;
            float _InRadius;


            inline float random(float2 uv)
            {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

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

            float3 GetPosWS(float2 screenUV)
            {
                float depth_o = SAMPLE_TEXTURE2D_X(
                    _CameraDepthTexture, sampler_CameraDepthTexture, screenUV).r;;


                //把ndc坐标转换到世界空间
                float4 ndc = float4(screenUV * 2 - 1, (1 - depth_o) * 2 - 1, 1);
                float4 posWS = mul(_VPMatrix_invers, ndc);
                posWS /= posWS.w;

                return posWS.xyz;
            }
            


            float4 Frag(Varyings i) : SV_Target
            {
                float2 screenUV = i.positionCS.xy / (_ScaledScreenParams.xy);
                float3 baseColor = SAMPLE_TEXTURE2D(_CamColorTexture,
                                                    sampler_CamColorTexture, screenUV).rgb;
                float3 ObjPosWS = GetPosWS(screenUV);

                float d = distance(ObjPosWS, _PlayerPos);
                float val = saturate((d - _InRadius) / (_OutRadius - _InRadius));
                // float d2 = distance(i.posWS, _PlayerPos);
                // float val2 = saturate((d2 - _InRadius) / (_OutRadius - _InRadius));
                val = smoothstep(0, 1, val);
                //return smoothstep(0,1,val*val2) ;


                float2 uv = ObjPosWS.xz * 0.1 + _WindDir.xz * _Time.y * _WindDir.w;

                float fog = SAMPLE_TEXTURE2D(_FogNoiseTex2, sampler_FogNoiseTex2, uv);


                float3 finColor = lerp(baseColor * _MaxInt, _MinInt, fog);
                finColor = lerp(baseColor, finColor, val);
                return float4(finColor, 1);
            }
            ENDHLSL
        }
    }
}
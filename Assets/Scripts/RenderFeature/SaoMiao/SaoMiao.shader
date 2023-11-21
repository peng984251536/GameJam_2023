Shader "SaoMiaoShader"
{

    Properties
    {
        //_DecalTex("_DecalTex",2D) = "while"{}
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    float4 _Sensitivity;

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

    //检测是否相同
    float CheckSame(float3 normal1, float depth1,
                    float3 normal2, float depth2)
    {
        float diffNormal = (1 - dot(normal1, normal2)) * _Sensitivity.x;
        int isSameNormal = diffNormal < _Sensitivity.z;

        float diffDepth = abs(depth1 - depth2) * _Sensitivity.y;
        int isSameDepth = diffDepth < _Sensitivity.w * depth1;

        return isSameNormal * isSameDepth;
        return isSameNormal * isSameDepth ? 1.0 : 0.0;
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }
        //        Cull Off 
        //        ZWrite Off 
        //        ZTest LEqual

        Pass
        {
            Name "MyVolumeFog"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            ZTest Always
            ZWrite Off
            Cull Off

            Stencil
            {
                Ref 1
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
            TEXTURE2D_X(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);
            TEXTURE2D(_CameraTex);
            SAMPLER(sampler_CameraTex);

            float4x4 _VPMatrix_invers;
            float4x4 _PMatrix_invers;
            float4x4 _VMatrix_invers;
            float4x4 _VMatrix;
            float4x4 _PMatrix;

            float4 _CameraTex_TexelSize;
            float3 _Color;
            float3 _EdgeColor;
            float3 _BackgroundColor;
            float _SaoMiaoDistance;
            float _Width;
            float3 _PlayerPos;


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

            inline float random(float2 uv)
            {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float GetLineDepth(float2 uv)
            {
                float depth_o = SAMPLE_TEXTURE2D_X(
                    _CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float linearDepth = LinearEyeDepth(depth_o, _ZBufferParams);
                return linearDepth;
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
                float3 baseColor = SAMPLE_TEXTURE2D(_CameraTex,
                                                    sampler_CameraTex, screenUV).rgb;
                float depth_o = SAMPLE_TEXTURE2D_X(
                    _CameraDepthTexture, sampler_CameraDepthTexture, screenUV).r;
                float3 posWS = GetPosWS(screenUV);
                float noise = SAMPLE_TEXTURE2D_X(
                    _NoiseTexture, sampler_NoiseTexture, posWS.xz*0.1).r;
                //return float4(posWS,1);

                float d = distance(posWS, _PlayerPos);
                float linearDepth = LinearEyeDepth(depth_o, _ZBufferParams);

                //基于深度和法线的边缘检测
                float2 screenUV1 = screenUV + _CameraTex_TexelSize.xy * float2(1, 1);
                float2 screenUV2 = screenUV + _CameraTex_TexelSize.xy * float2(-1, -1);
                float2 screenUV3 = screenUV + _CameraTex_TexelSize.xy * float2(-1, 1);
                float2 screenUV4 = screenUV + _CameraTex_TexelSize.xy * float2(1, -1);
                float depth1 = GetLineDepth(screenUV1);
                float depth2 = GetLineDepth(screenUV2);
                float depth3 = GetLineDepth(screenUV3);
                float depth4 = GetLineDepth(screenUV4);
                float3 normal = SampleSceneNormals(screenUV);
                float3 normal1 = SampleSceneNormals(screenUV);
                float3 normal2 = SampleSceneNormals(screenUV);
                float3 normal3 = SampleSceneNormals(screenUV);
                float3 normal4 = SampleSceneNormals(screenUV);
                //本质上就是判断对角的两个片元对应的法线和深度是否相近，
                //不相近则视为这个片元是边界
                float edge = 1.0;
                edge *= CheckSame(normal1, depth1, normal2, depth2);
                edge *= CheckSame(normal3, depth3, normal4, depth4);
                float3 finalColor = lerp(_EdgeColor, _BackgroundColor * baseColor, edge);
                //return smoothstep(0,1,dot(normal1,normal2)) *_Sensitivity.x;
                //return float4(finalColor, 1);


                //_SaoMiaoDistance
                float scanPercent = sin((d - _SaoMiaoDistance * 0.08) * _Width) + 1;
                scanPercent /= 2;
                scanPercent = smoothstep(0.95f, 1, scanPercent);
                //scanPercent = min()
                finalColor = lerp(finalColor, _Color.rgb, scanPercent);
                finalColor = lerp(finalColor, baseColor, linearDepth > (1 / _ZBufferParams.w - 2));

                // float scanPercent = 1 - abs(d - 0.5 * _Width) / _Width;
                // scanPercent = smoothstep(0.55, 1, scanPercent);
                // finalColor = lerp(finalColor, _Color.rgb, scanPercent);


                return float4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}
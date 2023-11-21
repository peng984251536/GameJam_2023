using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class TransFeature : ScriptableRendererFeature
{
    public TransSettings renderSettings;
    public Material effectMat;

    private TransPass renderPass;


    public override void Create()
    {
        OnCreate();
    }

    protected override void Dispose(bool disposing)
    {
        if (renderPass != null)
        {
            renderPass.OnDestroy();
            renderPass = null;
        }
    }

    public void OnCreate()
    {
        if (renderPass == null)
        {
            renderPass = new TransPass();
        }


        renderPass.OnInit(effectMat, renderSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (effectMat == null)
        {
            return;
        }

        if (!GamePlayInfo.isOpenTransition)
        {
            return;
        }

        if (renderingData.cameraData.camera.name == "Main Camera")
        {
            renderer.EnqueuePass(renderPass);
        }
    }


    #region 拉的平面反射代码

    //
    //  [Serializable]
    // public enum ResolutionMulltiplier
    // {
    //     Full,
    //     Half,
    //     Third,
    //     Quarter
    // }
    //
    // //关于平面反射的设置
    // [Serializable]
    // public class PlanarReflectionSettings
    // {
    //     public ResolutionMulltiplier m_ResolutionMultiplier = ResolutionMulltiplier.Third;
    //     public float m_planeOffset;
    //     public float m_planeNearOffset;
    //     public float m_ClipPlaneOffset = 0.1f;
    //     //public LayerMask m_ReflectLayers = -1;
    //     public bool m_Shadows;
    // }
    //
    //
    // private static Camera _reflectionCamera;
    // private RenderTexture _reflectionTexture;
    // private readonly int _planarReflectionTextureId = Shader.PropertyToID("_PlanarReflectionTexture");
    //
    // public PlanarReflectionSettings settings = new PlanarReflectionSettings();
    // public GameObject planer;
    //
    // // private void OnEnable()
    // // {
    // //     RenderPipelineManager.beginCameraRendering += ExecutePlanarReflections;
    // // }
    //
    // // private void OnDisable()
    // // {
    // //     Cleanup();
    // // }
    // //
    // // private void OnDestroy()
    // // {
    // //     Cleanup();
    // // }
    //
    // //关闭时调用这个函数清除不需要的对象
    // private void Cleanup()
    // {
    //     //RenderPipelineManager.beginCameraRendering -= ExecutePlanarReflections;
    //
    //     if (_reflectionCamera)
    //     {
    //         _reflectionCamera.targetTexture = null;
    //         if (Application.isEditor)
    //         {
    //             DestroyImmediate(_reflectionCamera.gameObject);
    //         }
    //         else
    //         {
    //             Destroy(_reflectionCamera.gameObject);
    //         }
    //     }
    //
    //     if (_reflectionTexture)
    //     {
    //         RenderTexture.ReleaseTemporary(_reflectionTexture);
    //     }
    // }
    //
    // /// <summary>
    // /// 虚拟摄像机开始工作
    // /// </summary>
    // /// <param name="context"></param>
    // /// <param name="camera"></param>
    // private void ExecutePlanarReflections(ScriptableRenderContext context, Camera camera)
    // {
    //     UpdateReflectionCamera(camera);
    //     
    //     //优化方案 - 降低系统的渲染质量，渲染完图片后再恢复原来的渲染设置
    //      var data =
    //          new PlanarReflectionSettingData();
    //      data.Set(); // set quality settings
    //
    //      //启动关键字？
    //     Shader.EnableKeyword("_PLANAR_REFLECTION_CAMERA");
    //     // render planar reflections
    //     UniversalRenderPipeline.RenderSingleCamera(context, _reflectionCamera);
    //     
    //     data.Restore(); // restore the quality settings
    //     
    //     Shader.SetGlobalTexture(_planarReflectionTextureId, _reflectionTexture); // Assign texture to water shader
    //     Shader.DisableKeyword("_PLANAR_REFLECTION_CAMERA");
    // }
    //
    //
    // /// <summary>
    // /// 更新反射贴图的摄像机
    // /// 输入的是真实的摄像机
    // /// </summary>
    // /// <param name="realCamera"></param>
    // private void UpdateReflectionCamera(Camera realCamera)
    // {
    //     if (_reflectionCamera == null)
    //         _reflectionCamera = CreateMirrorObjects(realCamera);
    //     
    //     UpdateCamera(realCamera, _reflectionCamera);
    //
    //     Vector3 camPos = _reflectionCamera.transform.position;
    //     //修改虚拟摄像机的位置
    //     if (PlayerInputManager.Instance != null)
    //     {
    //         Vector3 playPos = PlayerInputManager.Instance.transform.position;
    //         if (playPos.y > 50 && playPos.y < 150)
    //         {
    //             _reflectionCamera.transform.position = new Vector3(camPos.x, camPos.y-100, camPos.z);
    //         }
    //         else
    //         {
    //             _reflectionCamera.transform.position = new Vector3(camPos.x, camPos.y+100, camPos.z);
    //         }
    //         
    //     } 
    //     
    //     
    //     //设置渲染RT
    //     if (_reflectionTexture == null)
    //     {
    //         //渲染精度
    //         float scale = UniversalRenderPipeline.asset.renderScale;
    //         var width = (int) (realCamera.pixelWidth * scale * GetScaleValue());
    //         var height = (int) (realCamera.pixelHeight * scale * GetScaleValue());
    //         bool useHdr10 = RenderingUtils.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float);
    //         RenderTextureFormat hdrFormat =
    //             useHdr10 ? RenderTextureFormat.RGB111110Float : RenderTextureFormat.DefaultHDR;
    //         _reflectionTexture = RenderTexture.GetTemporary(width, height, 16,
    //             GraphicsFormatUtility.GetGraphicsFormat(hdrFormat, true));
    //     }
    //     _reflectionCamera.targetTexture = _reflectionTexture;
    //
    // }
    //
    //
    // /// <summary>
    // /// 创建虚拟摄像机
    // /// </summary>
    // /// <returns></returns>
    // private Camera CreateMirrorObjects(Camera realCamera)
    // {
    //     var go = new GameObject("Planar Reflections", typeof(Camera));
    //     var cameraData = go.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;
    //
    //     cameraData.requiresColorOption = CameraOverrideOption.Off;
    //     cameraData.requiresDepthOption = CameraOverrideOption.Off;
    //     // var asset = UniversalRenderPipeline.asset;
    //     // var render = UniversalRenderPipeline.asset.scriptableRenderer;
    //     //设置这个URP相机的渲染层级
    //     cameraData.SetRenderer(3);
    //
    //     //设置这个虚拟相机的位置
    //     var t = realCamera.transform;
    //     var reflectionCamera = go.GetComponent<Camera>();
    //     reflectionCamera.transform.SetPositionAndRotation
    //         (t.position, t.rotation);
    //     reflectionCamera.depth = -10;
    //     reflectionCamera.enabled = false;
    //     go.hideFlags = HideFlags.DontSave;
    //
    //     return reflectionCamera;
    // }
    //
    // /// <summary>
    // /// 更新摄像机的信息，让其与真实摄像机同步
    // /// </summary>
    // /// <param name="src"></param>
    // /// <param name="dest"></param>
    // private void UpdateCamera(Camera src, Camera dest)
    // {
    //     if (dest == null) return;
    //
    //     dest.CopyFrom(src);
    //     dest.useOcclusionCulling = false;
    //     if (dest.gameObject.TryGetComponent(out UniversalAdditionalCameraData camData))
    //     {
    //         // turn off shadows for the reflection camera
    //         camData.renderShadows = settings.m_Shadows;
    //     }
    // }
    //
    // private float GetScaleValue()
    // {
    //     switch (settings.m_ResolutionMultiplier)
    //     {
    //         case ResolutionMulltiplier.Full:
    //             return 1f;
    //         case ResolutionMulltiplier.Half:
    //             return 0.75f;
    //         case ResolutionMulltiplier.Third:
    //             return 0.5f;
    //         case ResolutionMulltiplier.Quarter:
    //             return 0.25f;
    //         default:
    //             return 0.5f; // default to half res
    //     }
    // }
    //
    // class PlanarReflectionSettingData
    // {
    //     private readonly bool _fog;
    //     private readonly int _maxLod;
    //     private readonly float _lodBias;
    //
    //     public PlanarReflectionSettingData()
    //     {
    //         _fog = RenderSettings.fog;
    //         _maxLod = QualitySettings.maximumLODLevel;
    //         _lodBias = QualitySettings.lodBias;
    //     }
    //
    //     public void Set()
    //     {
    //         //进行反转剔除
    //         GL.invertCulling = true;
    //         RenderSettings.fog = false; // disable fog for now as it's incorrect with projection
    //         QualitySettings.maximumLODLevel = 1;
    //         QualitySettings.lodBias = _lodBias * 0.5f;
    //     }
    //
    //     public void Restore()
    //     {
    //         GL.invertCulling = false;
    //         RenderSettings.fog = _fog;
    //         QualitySettings.maximumLODLevel = _maxLod;
    //         QualitySettings.lodBias = _lodBias;
    //     }
    // }
    //

    #endregion
}
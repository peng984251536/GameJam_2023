using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//[DefaultExecutionOrder(1)]
public class CameraManager : MonoSingleton<CameraManager>
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    private readonly int _CopyCameraTextureId = Shader.PropertyToID("_CopyCameraTexture");
    public RenderTexture cameraTexture;

    protected override void OnStart()
    {
        base.OnStart();
        //RenderPipelineManager.endCameraRendering += ExecutePlanarReflections;
    }

    private void LateUpdate()
    {
        if(!Application.isPlaying)
            return;
    
        // 计算摄像机的目标位置
        //Vector3 desiredPosition = target.position + offset;
        
        

        // 使用插值平滑移动摄像机
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 设置摄像机的位置
        transform.position = GamePlayInfo.cameraPos;
        // Vector3 camDir = transform.forward;
        // Vector3 targetDir = target.forward;
        // transform.forward = new Vector3(targetDir.x, camDir.y, targetDir.z);
        // this.transform.position = target.transform.position;
        // this.transform.rotation = target.transform.rotation;

        // 让摄像机始终面向玩家
        transform.LookAt(target);
    }


    public void UpdateCameraPos(Vector3 pos)
    {
        if(!Application.isPlaying)
            return;
    
        // 计算摄像机的目标位置
        //Vector3 desiredPosition = target.position + offset;
        
        

        // 使用插值平滑移动摄像机
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 设置摄像机的位置
        transform.position = pos;
        // Vector3 camDir = transform.forward;
        // Vector3 targetDir = target.forward;
        // transform.forward = new Vector3(targetDir.x, camDir.y, targetDir.z);
        // this.transform.position = target.transform.position;
        // this.transform.rotation = target.transform.rotation;

        // 让摄像机始终面向玩家
        transform.LookAt(target);
    }

    private void ExecutePlanarReflections(ScriptableRenderContext arg1, Camera arg2)
    {
        //设置渲染RT
        if (cameraTexture == null)
        {
            //渲染精度
            float scale = UniversalRenderPipeline.asset.renderScale;
            var width = (int) (Screen.width * scale * 0.5f);
            var height = (int) (Screen.height * scale * 0.5f);
            cameraTexture = RenderTexture.GetTemporary(width, height, 16,
                GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.ARGB1555, true));
        }

        arg2.targetTexture = cameraTexture;
        
        Shader.SetGlobalTexture(_CopyCameraTextureId, cameraTexture);
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    public void Cleanup()
    {
        
        RenderPipelineManager.beginCameraRendering -= ExecutePlanarReflections;
        

        if (cameraTexture)
        {
            RenderTexture.ReleaseTemporary(cameraTexture);
        }
    }

    public void ReStart()
    {
        RenderPipelineManager.endCameraRendering += ExecutePlanarReflections;
    }
}
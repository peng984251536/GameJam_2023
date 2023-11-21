using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MistyPass : ScriptableRenderPass
{
    private const string k_tag = "MistyPass";
    private Material effectMat;
    

    public MistyPass()
    {
        profilingSampler = new ProfilingSampler(k_tag);
    }

    public void OnInit(Material _effectMat)
    {
        effectMat = _effectMat;
    }

    public void OnDestroy()
    {
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        //告诉URP我们需要深度和法线贴图
        ConfigureInput(ScriptableRenderPassInput.Depth);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //用于矩阵转换的参数
        Camera cam = renderingData.cameraData.camera;
        Matrix4x4 p_Matrix = cam.projectionMatrix;
        Matrix4x4 v_Matrix = cam.worldToCameraMatrix;
        Matrix4x4 vp_Matrix = cam.projectionMatrix * cam.worldToCameraMatrix;
        effectMat.SetMatrix("_VPMatrix_invers", vp_Matrix.inverse);
        effectMat.SetMatrix("_PMatrix_invers", p_Matrix.inverse);
        effectMat.SetMatrix("_VMatrix_invers", v_Matrix.inverse);
        effectMat.SetMatrix("_VMatrix", v_Matrix);
        effectMat.SetMatrix("_PMatrix", p_Matrix);

        ConfigureClear(ClearFlag.None, Color.white);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, profilingSampler))
        {
            RenderTargetIdentifier cameraRT =  renderingData.cameraData.renderer.cameraColorTarget;
            
            //拷贝
            RenderTargetHandle rt = new RenderTargetHandle();
            rt.Init("_CamColorTexture");
            // RenderTargetHandle rt2 = new RenderTargetHandle();
            // rt2.Init("_MyBaseMap");
            RenderTextureDescriptor baseDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            baseDescriptor.depthBufferBits = 0;
            baseDescriptor.msaaSamples = 1;
            cmd.GetTemporaryRT(rt.id,baseDescriptor , FilterMode.Bilinear);
            cmd.Blit(cameraRT,rt.Identifier());
            
            // //混合输出
            cmd.SetGlobalTexture("_CamColorTexture",rt.Identifier());
            cmd.Blit(rt.Identifier(), cameraRT,effectMat,0);
            //释放这张图
            cmd.ReleaseTemporaryRT(rt.id);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
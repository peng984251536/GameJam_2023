using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransPass : ScriptableRenderPass

{
    private const string k_tag = "Transition";

    private TransSettings settings;
    private Material effectMat;


    public TransPass()
    {
        profilingSampler = new ProfilingSampler(k_tag);
    }

    public void OnInit(Material _effectMat, TransSettings _renderSettings)
    {
        effectMat = _effectMat;
        settings = _renderSettings;
        renderPassEvent = _renderSettings.renderPassEvent;
    }


    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        //告诉URP我们需要深度
        ConfigureInput(ScriptableRenderPassInput.None);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //用于矩阵转换的参数
        Camera cam = renderingData.cameraData.camera;
        Matrix4x4 vp_Matrix = cam.projectionMatrix * cam.worldToCameraMatrix;
        effectMat.SetMatrix("_VPMatrix_invers", vp_Matrix.inverse);
        effectMat.SetFloat("_Width", settings.width);
        effectMat.SetTexture("_NoiseTexture", settings.noiseTex);

        ConfigureClear(ClearFlag.None, Color.white);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, profilingSampler))
        {
            RenderTargetIdentifier cameraRT = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTargetHandle OpaqueTexture = new RenderTargetHandle();
            OpaqueTexture.Init("_CameraOpaqueTexture");
            // RenderTextureDescriptor baseDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            // baseDescriptor.useMipMap = false;
            // baseDescriptor.autoGenerateMips = false;
            // baseDescriptor.depthBufferBits = 0;
            // baseDescriptor.msaaSamples = 1;
            // cmd.GetTemporaryRT(rt.id, baseDescriptor, FilterMode.Bilinear);
            // cmd.Blit(cameraRT, rt.Identifier());
            // cmd.SetGlobalTexture("_CameraTex", rt.Identifier());

            //混合输出
            cmd.Blit(OpaqueTexture.Identifier(), cameraRT, effectMat, 0);

            //计算完后释放RT
            //cmd.ReleaseTemporaryRT(rt.id);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    

    public void OnDestroy()
    {
    }
}
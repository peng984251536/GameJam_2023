using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SaoMiaoPass : ScriptableRenderPass

{
    private const string k_tag = "SaoMiaoPass";

    private static readonly int noiseCB_ID = Shader.PropertyToID("_NoiseTexture");
    private static readonly int intensity_ID = Shader.PropertyToID("_Intensity");


    private SaoMiaoSettings settings;
    private Material effectMat;


    public SaoMiaoPass()
    {
        profilingSampler = new ProfilingSampler(k_tag);
    }

    public void OnInit(Material _effectMat, SaoMiaoSettings _renderSettings)
    {
        effectMat = _effectMat;
        settings = _renderSettings;
    }

    public void OnDestroy()
    {
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        //告诉URP我们需要深度
        ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
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
        effectMat.SetTexture(noiseCB_ID, settings.noiseTex);
        
        effectMat.SetFloat("_Width", settings.width);
        effectMat.SetColor("_Color", settings.saoMiaoColor);
        effectMat.SetColor("_EdgeColor", settings.edgeColor);
        effectMat.SetColor("_BackgroundColor", settings.bColor);
        effectMat.SetVector("_Sensitivity",settings.sensitivity);

        ConfigureClear(ClearFlag.None, Color.white);
}

public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
{
    var cmd = CommandBufferPool.Get();
    using (new ProfilingScope(cmd, profilingSampler))
    {
        RenderTargetIdentifier cameraRT = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTargetHandle rt = new RenderTargetHandle();
        rt.Init("_MyCameraColor");
        RenderTextureDescriptor baseDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        baseDescriptor.useMipMap = false;
        baseDescriptor.autoGenerateMips = false;
        baseDescriptor.depthBufferBits = 0;
        baseDescriptor.msaaSamples = 1;
        cmd.GetTemporaryRT(rt.id, baseDescriptor, FilterMode.Bilinear);
        cmd.Blit(cameraRT, rt.Identifier());
        cmd.SetGlobalTexture("_CameraTex", rt.Identifier());

        //混合输出
        cmd.Blit(rt.Identifier(), cameraRT, effectMat, 0);

        //计算完后释放RT
        cmd.ReleaseTemporaryRT(rt.id);
    }

    context.ExecuteCommandBuffer(cmd);
    CommandBufferPool.Release(cmd);
}


/// <summary>
/// 生产噪点
/// </summary>
/// <returns></returns>
private Vector2[] GenerateNoise()
{
    Vector2[] noises = new Vector2[4 * 4];

    for (int i = 0; i < noises.Length; i++)
    {
        float x = Random.value;
        float y = Random.value;
        noises[i] = new Vector2(x, y);
    }

    return noises;
}

private Vector4[] _noises;

private Vector4[] GenerateNoise(bool isVector4)
{
    if (_noises != null)
    {
        return _noises;
    }

    Vector4[] noises = new Vector4[4 * 4];

    for (int i = 0; i < noises.Length; i++)
    {
        float x = Random.value;
        float y = Random.value;
        noises[i] = new Vector4(x, y, 0, 0);
    }

    _noises = noises;
    return noises;
}
}
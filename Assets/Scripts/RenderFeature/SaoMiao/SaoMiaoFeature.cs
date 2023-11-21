using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SaoMiaoFeature : ScriptableRendererFeature
{
    public SaoMiaoSettings renderSettings;
    public Material effectMat;


    private SaoMiaoPass renderPass;


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
            renderPass = new SaoMiaoPass()
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing,
            };
        }


        renderPass.OnInit(effectMat, renderSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (effectMat == null)
        {
            return;
        }

        if (PlayerDataManager.Instance!=null&&PlayerDataManager.Instance.isOpenSMFrature)
        {
            renderer.EnqueuePass(renderPass);            
        }
        //renderer.EnqueuePass(renderPass); 

    }
}
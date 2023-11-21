using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MistyFeature : ScriptableRendererFeature
{
    public Material effectMat;
    
    private MistyPass renderPass;
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;


    public override void Create()
    {
        if (renderPass == null)
        {
            renderPass = new MistyPass()
            {
                renderPassEvent = this.renderPassEvent,
            };
        }


        renderPass.OnInit(effectMat);
    }

    protected override void Dispose(bool disposing)
    {
        if (renderPass != null)
        {
            renderPass.OnDestroy();
            renderPass = null;
        }
    }
    

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (effectMat == null)
        {
            return;
        }
        
        if (renderingData.cameraData.camera.name == "Main Camera"&& Application.isPlaying)
        {
            renderer.EnqueuePass(renderPass);
        }
    }
    
}
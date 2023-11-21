using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class TransSettings
{
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRendering;
    public Texture noiseTex;
    
    public float width = 1.0f;
    //public float length = 1.0f;
}
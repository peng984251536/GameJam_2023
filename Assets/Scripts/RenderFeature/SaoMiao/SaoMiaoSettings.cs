using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class SaoMiaoSettings
{
    public Texture noiseTex;

    public Color saoMiaoColor = Color.grey;
    public Color edgeColor;
    public Color bColor;

    public float width = 1.0f;

    public Vector4 sensitivity = new Vector4(1.0f, 1.0f, 0.1f, 0.1f);
}
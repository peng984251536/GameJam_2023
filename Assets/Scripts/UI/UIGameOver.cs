using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoSingleton<UIGameOver>
{
    public Button button;

    public RawImage BG;
    public Image deadImg;
    public Image wimImg;

    protected override void OnStart()
    {
    }

    public void OnClickRe()
    {
        UIManager.Instance.Close(typeof(UIGameOver));
        UIManager.Instance.Show<UIGameStart>();

        PlayerInputManager.Instance.reBackEvent();
        MusicManager.Instance.playMusic(AudioData.previewMusic);
    }

    public void OnShowDead(bool isShow)
    {
        if(isShow)
            MusicManager.Instance.playSound(AudioData.failure);
        
        deadImg.gameObject.SetActive(isShow);
        deadImg.enabled = isShow;
        BG.texture = CameraManager.Instance.cameraTexture;
        CameraManager.Instance.Cleanup();
    }

    public void OnShowWin(bool isShow)
    {
        if(isShow)
            MusicManager.Instance.playSound(AudioData.succeed);
        wimImg.gameObject.SetActive(isShow);
        wimImg.enabled = isShow;
        
        BG.texture = CameraManager.Instance.cameraTexture;
        CameraManager.Instance.Cleanup();
    }
}
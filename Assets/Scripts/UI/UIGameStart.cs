using UnityEngine;

public class UIGameStart : MonoSingleton<UIGameStart>
{

    protected override void OnStart()
    {
        
    }
    
    public void OnClickGameStart()
    {
        UIManager.Instance.Close(typeof(UIGameStart));
        UIManager.Instance.Show<UIGamePlay>();
        UIGamePlay.Instance.SetProgressText(0);
        MusicManager.Instance.stopMusic(AudioData.previewMusic);
        
        MusicItem[] musicItems = GameObject.FindObjectsOfType<MusicItem>();
        for (int i = 0; i < musicItems.Length; i++)
        {
            musicItems[i].StartPlay();
        }
    }
}
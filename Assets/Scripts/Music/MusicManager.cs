using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class MusicManager : MonoSingleton<MusicManager>
{
    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>(10);

    public AudioSource musicSource;
    public AudioSource soundSource;
    private string soundAudioPath = "Sound/";
    private string musicAudioPath = "Music/";

    protected override void OnStart()
    {
        InitAudio();
    }

    private void InitAudio()
    {
        LoadSoundAudio(AudioData.saomiao);
        LoadSoundAudio(AudioData.touchInWater);
        LoadSoundAudio(AudioData.collect);
        LoadSoundAudio(AudioData.failure);
        LoadSoundAudio(AudioData.succeed);

        LoadMusicAudio(AudioData.item_a);
        LoadMusicAudio(AudioData.item_b);
        LoadMusicAudio(AudioData.item_c);
        LoadMusicAudio(AudioData.item_d);
        LoadMusicAudio(AudioData.previewMusic);

        // StartCoroutine(LoadAsyncSoundAudio(AudioData.item_a));
        // StartCoroutine(LoadAsyncSoundAudio(AudioData.item_b));
        // StartCoroutine(LoadAsyncSoundAudio(AudioData.item_c));
        // StartCoroutine(LoadAsyncSoundAudio(AudioData.item_d));
        musicSource.loop = true;
    }

    public void LoadMusicAudio(string name)
    {
        // 加载音效文件
        //Resources.Load<AudioClip>("Audio/myAudio")
        AudioClip audioClip = Resources.Load<AudioClip>(musicAudioPath + name);
        audioClips.Add(name, audioClip);
    }

    public void LoadSoundAudio(string name)
    {
        // 加载音效文件
        //Resources.Load<AudioClip>("Audio/myAudio")
        AudioClip audioClip = Resources.Load<AudioClip>(soundAudioPath + name);
        audioClips.Add(name, audioClip);
    }

    IEnumerator LoadAsyncSoundAudio(string name)
    {
        // 加载音效文件
        //Resources.Load<AudioClip>("Audio/myAudio")
        ResourceRequest request = Resources.LoadAsync<AudioClip>(soundAudioPath + name);

        yield return request;

        //资源加载完成
        Debug.LogFormat("music:{0} is load", name);
        AudioClip audioClip = request.asset as AudioClip;
        audioClips.Add(name, audioClip);
    }

    public void playMusic(string name,float volume=0.5f)
    {
        AudioClip audioClip;
        audioClips.TryGetValue(name, out audioClip);
        if (audioClip != null)
        {
            musicSource.clip = audioClip;
            musicSource.volume = volume;
            musicSource.Play();
        }
    }

    public void playMusic(string name, AudioSource musicSource)
    {
        AudioClip audioClip;
        audioClips.TryGetValue(name, out audioClip);
        if (audioClip != null)
        {
            //musicSource.PlayScheduled();
            musicSource.clip = audioClip;
            musicSource.Play();
        }
    }

    public void playSound(string name,float v=0.7f)
    {
        AudioClip audioClip;
        audioClips.TryGetValue(name, out audioClip);
        soundSource.volume = v;
        if (audioClip != null)
        {
            soundSource.clip = audioClip;
            soundSource.Play();
        }
    }

    public void playSound(string name, AudioSource soundSource)
    {
        AudioClip audioClip;
        audioClips.TryGetValue(name, out audioClip);
        if (audioClip != null)
        {
            soundSource.clip = audioClip;
            soundSource.Play();
        }
    }

    public void stopMusic(string name)
    {
        AudioClip audioClip;
        audioClips.TryGetValue(name, out audioClip);
        if (audioClip != null && musicSource.clip == audioClip)
        {
            Debug.Log("stop music:" + name);
            musicSource.clip = null;
            musicSource.Stop();
        }
    }
}
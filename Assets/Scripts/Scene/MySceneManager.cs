using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoSingleton<MySceneManager>
{
    public UnityAction<float> onProgress = null;

    // Use this for initialization
    protected override void OnStart()
    {
        
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void LoadScene(string name)
    {
        StartCoroutine(LoadLevel(name));
    }

    //加载不在Resource的场景
    public void LoadOutScene(string name)
    {
        StartCoroutine(LoadSceneAsync(name));
    }

    IEnumerator LoadLevel(string name)
    {
        Debug.LogFormat("LoadLevel: {0}", name);
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = true;
        async.completed += LevelLoadCompleted;
        while (!async.isDone)
        {
            if (onProgress != null)
                onProgress(async.progress);
            yield return null;
        }
    }
    
    private IEnumerator LoadSceneAsync(string name)
    {
        // 加载 AssetBundle
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(name);
        yield return bundleRequest;

        // 获取 AssetBundle
        AssetBundle bundle = bundleRequest.assetBundle;

        // 加载场景
        AsyncOperation async = SceneManager.
            LoadSceneAsync("YourSceneName", LoadSceneMode.Single);
        async.allowSceneActivation = true;
        async.completed += LevelLoadCompleted;
        while (!async.isDone)
        {
            if (onProgress != null)
                onProgress(async.progress);
            yield return null;
        }

        // 卸载 AssetBundle
        bundle.Unload(false);
    }

    private void LevelLoadCompleted(AsyncOperation obj)
    {
        if (onProgress != null)
            onProgress(1f);
        Debug.Log("LevelLoadCompleted:" + obj.progress);
    }
}

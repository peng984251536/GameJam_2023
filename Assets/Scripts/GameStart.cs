using System;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameObject player;
    public Transform startPos;

    private GameObject _playerGameObject;

    private void Awake()
    {

        GameObject go;
        if (startPos != null)
        {
            go = Instantiate(player);
            _playerGameObject = go;
            var position = startPos.position;
            go.transform.position = position;
            PlayerDataManager.Instance.originalPos = position;
            PlayerDataManager.Instance.originalDir = startPos.forward;
        }
        else
        {
            go = Instantiate(player);
        }

        //不能用单例
        CameraManager cam = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        cam.target = go.transform;

        //MySceneManager.Instance.LoadScene(SceneData.CoralReef);
        //MySceneManager.Instance.onProgress += DebugPre;
    }

    private void Start()
    {
        UIManager.Instance.Show<UIGameStart>();
        MusicManager.Instance.playMusic(AudioData.previewMusic);
    }

    private void OnDestroy()
    {
        Destroy(_playerGameObject);
    }

    public void DebugPre(float progress)
    {
        Debug.Log("当前进度：" + progress);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class PlayerDataManager : MonoSingleton<PlayerDataManager>
{
    [HideInInspector]
    public PlayerInputManager.MoveState moveState = PlayerInputManager.MoveState.None;
    public Vector2 playerMovePos = Vector2.one;
    
    [HideInInspector]
    public float curRotateVal = 0.0f;
    public float curRotateSpeed = 1.0f;
    public int curClickCount = 0;
    
    //记录玩家收集的音符
    public Dictionary<MusicType, float> musicDataDic = new Dictionary<MusicType, float>();

    [Header("-----角色相关配置---------")]
    public float speed = 1.0f;
    public float stopSpeed = 0.5f;
    public float rotateSpeed = 1.0f;
    public float turnAngle = 10;
    public Vector3 camOffset = Vector3.zero;
    public float childFalllowDis = 2.0f;

    [Header("-----SaoMiao---------")]
    public float maxSaoMiaoSkillCD = 5.0f;
    public float cursmSkillCD = 0;
    public float saoMiaoSpeen = 0.5f;
    public float saomiaoDefauDistance = -26;
    public float saomiaoDistance = -26;
    public float saomiaoTime = 6.0f;
    public float maxDistance = 50;

    [Header("--------Run---------")]
    public float maxRunSkillCD = 2.0f;
    public float curRunSkillCD = 0;
    public float maxPlayTime = 15;
    public float maxPlayFrameCount = 45;
    public float curPlayFrameCount = 45;
    public float length = 0;
    public float RunSpeed = 8;
    public float currentWorld = 0;
    public GameObject[] world1;
    public GameObject[] world2;

    //-----------------------
    public Vector3 originalPos;
    public Vector3 originalDir;

    //--------渲染相关-----------
    [FormerlySerializedAs("isOpenSMFrature")]
    public bool isOpenSMFrature = false;

    public bool isOpenTransitionFrature = false;
    public int musicItemPassIndex = 1;

    public float mistyOutRadius = 10.39f;
    public float mistyInRadius = 3.39f;

    protected override void OnStart()
    {
        PlayerInputManager.Instance.reBackEvent += ReSetMusicItem;
    }

    


    
    private void Update()
    {

    }
    

    public void GetMusicItem(MusicType musicType)
    {
        musicDataDic.Add(musicType, 0.1f);
        UIGamePlay.Instance.SetProgressText(musicDataDic.Count);
        StartCoroutine(OpenGameOverUI());
    }

    IEnumerator OpenGameOverUI()
    {
        yield return new WaitForSecondsRealtime(5.0f);
        if (musicDataDic.Count >= 4)
        {
            UIManager.Instance.Close(typeof(UIGamePlay));
            UIManager.Instance.Show<UIGameOver>();
            UIGameOver.Instance.OnShowDead(false);
            UIGameOver.Instance.OnShowWin(true);
            Debug.Log("游戏胜利！");
            PlayerInputManager.Instance.OnPlayerMove(PlayerInputManager.MoveState.None, false);
        }

        yield break;
    }

    public void ReSetMusicItem()
    {
        musicDataDic.Clear();
    }

    private void FixedUpdate()
    {
        if (cursmSkillCD >= 0)
        {
            cursmSkillCD = Mathf.Max(0, cursmSkillCD - Time.fixedDeltaTime);
            musicItemPassIndex = cursmSkillCD == 0 ? 1 : 0;
        }

        if (curRunSkillCD >= 0)
        {
            curRunSkillCD = Mathf.Max(0, curRunSkillCD - Time.fixedDeltaTime);
        }

        if (UIGamePlay.Instance != null && UIGamePlay.Instance.enabled)
        {
            UIGamePlay.Instance.SetSmSkillCD(1 - cursmSkillCD / maxSaoMiaoSkillCD);
            UIGamePlay.Instance.SetRunSkillCD(1 - curRunSkillCD / maxRunSkillCD);
        }
        
        
    }

    public void SetWorldId(int id,bool isOpen)
    {
        if (id == 0)
        {
            for (int i = 0; i < world1.Length; i++)
            {
                world1[i].SetActive(isOpen);
            }
        }
        else if (id == 1)
        {
            for (int i = 0; i < world2.Length; i++)
            {
                world2[i].SetActive(isOpen);
            }
        }
    }
}
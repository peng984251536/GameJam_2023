using System;
using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum MusicType
{
    [Description("a_2")] a_2,
    [Description("b_2")] b_2,
    [Description("c_2")] c_2,
    [Description("d_2")] d_2
}

public class MusicItem : MonoBehaviour
{
    //PlayerInputManager playerMgr;
    PlayerDataManager playerData;
    public MusicType musicType = MusicType.a_2;
    public float distance = 5;


    private CapsuleCollider Collider;
    public AudioSource Source;
    private float TargetAudioVolume;
    private bool isBeEat = false;
    private GameObject parent;
    private Rigidbody rb;
    private bool isOver = false;
    private Vector3 originalPos;

    private void Start()
    {
        Collider = this.gameObject.GetComponent<CapsuleCollider>();
        Collider.radius = distance;

        Source = gameObject.GetComponent<AudioSource>();
        if (Source == null)
        {
            Source = gameObject.AddComponent<AudioSource>();
            Source.volume = 0.1f;
        }

        playerData = PlayerDataManager.Instance;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.Sleep();
        originalPos = transform.position;
        PlayerInputManager.Instance.reBackEvent += ResBack;
        TargetAudioVolume = 0.01f;

        //开始时播放音乐
        // MusicManager.Instance.playMusic(musicType.ToString(), Source);
        // Source.volume = 0.01f;
    }

    private void Update()
    {
        //Debug.Log("!!!music time :" + Source.time);
        if (Source.time >= playerData.maxPlayTime && isOver == false)
        {
            isOver = true;
            UIManager.Instance.Close(typeof(UIGamePlay));
            UIManager.Instance.Show<UIGameOver>();
            UIGameOver.Instance.OnShowDead(true);
            UIGameOver.Instance.OnShowWin(false);
            PlayerInputManager.Instance.OnPlayerMove(PlayerInputManager.MoveState.None, false);
            //UIGamePlay.Instance.SetTimeText(Source.time);
        }
        else
        {
            if (UIGamePlay.Instance != null)
                UIGamePlay.Instance.SetTimeText(playerData.maxPlayTime - Source.time);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (isBeEat)
            return;

        // 获取碰撞对象的层级ID
        int layerId = collision.gameObject.layer;
        GameObject player = collision.gameObject;

        // 将层级ID转换为名称
        string layerName = LayerMask.LayerToName(layerId);

        if (layerName == LayerData.player)
        {
            //Debug.LogFormat("Stay Name is {0}。goName is {1}", this.name, collision.gameObject.name);
            Vector3 pos1 = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 pos2 = new Vector3(this.transform.position.x, 0, this.transform.position.z);
            float d = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
            float voleme = Mathf.Abs((distance - d) / distance);
            TargetAudioVolume = Mathf.Max(0.01f, Mathf.Pow(voleme, 1 / 2.2f));
            //Source.volume = 
            //根据声道衰减声音
            Vector3 right = player.transform.right.normalized;
            Vector3 dir = (pos2 - pos1).normalized;
            float isRight = Vector3.Dot(right, dir);

            Source.panStereo = Mathf.Pow(isRight, 1 / 2.2f);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (isBeEat)
            return;

        // 获取碰撞对象的层级ID
        int layerId = collision.gameObject.layer;

        // 将层级ID转换为名称
        string layerName = LayerMask.LayerToName(layerId);
        //Debug.LogFormat("Exit LayerName is {0}。goName is {1}", layerName, collision.gameObject.name);

        if (layerName == LayerData.player)
        {
            //TODO --停止播放
            TargetAudioVolume = 0.01f;
            // if (Source.isPlaying)
            //     Source.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (Source.isPlaying && !isBeEat)
        {
            Source.volume = Mathf.Lerp(TargetAudioVolume, Source.volume, 0.5f);

            // if (Source.volume <= 0.02)
            // {
            //     Source.Stop();
            // }
        }

        //跟随逻辑
        if (isBeEat && parent != null)
        {
            Vector3 fallowPos = parent.transform.position -
                                parent.transform.forward * playerData.childFalllowDis;
            Vector3 dir = fallowPos - transform.position;
            transform.forward = Vector3.Lerp(
                transform.forward, parent.transform.forward, 0.5f);

            if (dir.magnitude <= 0.2)
            {
                rb.velocity = Vector3.zero;
                return;
            }

            rb.velocity = rb.velocity.y * transform.up +
                          dir.normalized * playerData.speed * 0.98f;
        }

        if (Source.volume >= 0.11 && !Source.isPlaying)
        {
            Debug.LogWarning("为什么音频被关掉了");
            Source.Play();
        }
    }

    /// <summary>
    /// 当物品被吃，上诉的功能都失效
    /// </summary>
    public void ItemBeEat(GameObject parent)
    {
        this.parent = parent;
        isBeEat = true;
        Source.volume = 0.3f;
        Source.panStereo = 0;
        //MusicManager.Instance.playMusic(musicType.ToString(), Source);

        rb.WakeUp();
        //----跟随-------------//
    }


    public void ResBack()
    {
        isBeEat = false;
        isOver = false;
        parent = null;
        rb.Sleep();
        Source.Stop();
        int layer = LayerMask.NameToLayer(LayerData.touch);
        this.gameObject.layer = layer;
        this.gameObject.transform.position = originalPos;
    }

    public void StartPlay()
    {
        //开始时播放音乐

        MusicManager.Instance.playMusic(musicType.ToString(), Source);
        Source.volume = 0.1f;
    }


    // IEnumerator FallowParent()
    // {
    //     yield return new WaitForSeconds(1.0f);
    //     
    //     this
    //     
    //     yield return null;
    // }
}
using System;
using System.Collections;
using System.Reflection;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameObjNode
{
    public GameObject go;
    public GameObjNode next;

    public GameObjNode(GameObject go, GameObjNode node)
    {
        this.go = go;
        this.next = node;
    }

    public GameObjNode(GameObject go)
    {
        this.go = go;
    }
}


//[ExecuteInEditMode]
public class PlayerInputManager : MonoSingleton<PlayerInputManager>
{
    public Action reBackEvent;
    public GameObject copyPlayer;

    private PlayerDataManager playerData;
    private Rigidbody rb;
    private GameObjNode gameObjNode;

    private bool isMouseDrag = false;
    private float timer = 0.0f;


    protected override void OnAwake()
    {
        base.OnAwake();
        playerData = PlayerDataManager.Instance;
    }

    protected override void OnStart()
    {
        rb = GetComponent<Rigidbody>();
        gameObjNode = new GameObjNode(this.gameObject);
        reBackEvent += OnClickBackInit;
        defaulSpeed = playerData.speed;
        defaulMistyOut = playerData.mistyOutRadius;
        defaulMistyIn = playerData.mistyInRadius;
        
        //playerData.SetWorldId(0, true);
        //playerData.SetWorldId(1, false);
        //Shader.SetGlobalFloat("_currentWorld",0);
    }

    private bool actionMouseButtonDown = false;
    private bool actionMouseButtonUp = false;

    private void Update()
    {
        Shader.SetGlobalVector("_PlayerPos", this.transform.position + transform.forward);
        Shader.SetGlobalFloat("_SaoMiaoDistance", playerData.saomiaoDistance);
        Shader.SetGlobalFloat("_OutRadius", playerData.mistyOutRadius);
        Shader.SetGlobalFloat("_InRadius", playerData.mistyInRadius);

        UpdateCopyPlayerY();
    }

    private void FixedUpdate()
    {
        // 获取玩家的输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = 0;
        // float mouseY = Input.GetAxisRaw("Mouse Y");
        if (Input.touchCount > 0)
        {
            for(int i = 0;i<Input.touchCount;i++)
            {
                Touch touch = Input.GetTouch(i);
                if (Vector2.Distance(touch.position, playerData.playerMovePos) <= 1.0f)
                {
                    Debug.Log("企图滑动移动按钮");
                    continue;
                }
                    
                mouseX = touch.deltaPosition.x/360*playerData.curRotateSpeed;
            }
        }
        //if(Input.touchCount)

        //------------角色移动-----------//
        if (PlayerDataManager.Instance.moveState==MoveState.None)
        {
            //前进-后退
            if (vertical > 0.01)
            {
                this.rb.velocity = this.rb.velocity.y * transform.up +
                                   transform.forward * playerData.speed;
            }
            else if (vertical < -0.01)
            {
                this.rb.velocity = this.rb.velocity.y * transform.up -
                                   transform.forward * playerData.speed;
            }
            else
            {
                this.rb.velocity = Vector3.Lerp(Vector3.zero, this.rb.velocity, playerData.stopSpeed);
            }
        }
        else
        {
            if (PlayerDataManager.Instance.moveState==MoveState.QianjingMove)
            {
                this.rb.velocity = this.rb.velocity.y * transform.up +
                                   transform.forward * playerData.speed;
            }
            else if (PlayerDataManager.Instance.moveState==MoveState.HoutuiMove)
            {
                this.rb.velocity = this.rb.velocity.y * transform.up -
                                   transform.forward * playerData.speed;
            }
        }


        //------------角色旋转-----------//
        if (horizontal < -0.1 || horizontal > 0.1)
        {
            Vector3 dir = new Vector3(0, horizontal * playerData.rotateSpeed, 0);
            this.transform.Rotate(dir);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > this.playerData.turnAngle && rot.eulerAngles.y < (360 - this.playerData.turnAngle))
            {
                rb.transform.forward = this.transform.forward;
                //this.SendEntityEvent(EntityEvent.None);
            }
        }
        //Debug.LogFormat("Input.touchCount:{0}",Input.touchCount);
        if (Mathf.Abs(mouseX) >=0.0001f)
        //else if (Mathf.Abs(playerData.curRotateVal) >= 0.000001f)
        {
            //Debug.LogFormat("curRotateVal:{0}",mouseX);
            float roateSpeed = (playerData.curRotateVal+mouseX)*playerData.rotateSpeed;
            Vector3 dir = new Vector3(0, roateSpeed, 0);
            this.transform.Rotate(dir);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > this.playerData.turnAngle && rot.eulerAngles.y < (360 - this.playerData.turnAngle))
            {
                rb.transform.forward = this.transform.forward;
                //this.SendEntityEvent(EntityEvent.None);
            }

            //playerData.curRotateVal = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q) && playerData.cursmSkillCD == 0)
        {
            OnClickSaoMiao();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            OnClickSaoMiao();
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerData.curRunSkillCD == 0)
        {
            OnClickRun();
        }

        // if (Input.GetMouseButtonDown(0))
        // {
        //     OnClickZhuanzhu();
        // }
        // else if (Input.GetMouseButtonUp(0))
        // {
        //     OnClickZhuanzhuBack();
        // }

        SetCameraPos();
        UpdateCopyPlayerXZ();

        if (playerData.cursmSkillCD != 0)
        {
            playerData.saomiaoDistance += playerData.saoMiaoSpeen;
            playerData.isOpenSMFrature = true;
        }
        else
        {
            playerData.saomiaoDistance = playerData.saomiaoDefauDistance;
            playerData.isOpenSMFrature = false;
        }

        playerData.curRotateVal = 0;
    }

    /// <summary>
    /// 碰撞到某个物体时，切换场景
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        // // 获取碰撞对象的层级ID
        // int layerId = collision.gameObject.layer;
        //
        // // 将层级ID转换为名称
        // string layerName = LayerMask.LayerToName(layerId);
        // Debug.LogFormat("LayerName is {0}。goName is {1}", layerName, collision.gameObject.name);
        //
        // if (layerName == LayerData.touch)
        // {
        //     Debug.Log("set itemMusic");
        //     Material material = collision.transform.GetComponent<MeshRenderer>().material;
        //     Color color = material.GetColor("_BaseColor");
        //     Shader.SetGlobalColor("_MusicItemColor",color);
        // }
    }


    private void OnTriggerStay(Collider collision)
    {
        // 获取碰撞对象的层级ID
        int layerId = collision.gameObject.layer;
        // 将层级ID转换为名称
        string layerName = LayerMask.LayerToName(layerId);
        //当玩家吃掉物体时调用

        if (layerName == LayerData.beEat)
            return;


        if (layerName == LayerData.touch)
        {
            GameObject item = collision.gameObject;
            float d = Vector3.Distance(item.transform.position,
                this.transform.position);
            if (d <= 3)
            {
                Debug.LogFormat("player ear is {0}", collision.gameObject.name);
                MusicManager.Instance.playSound(AudioData.collect, 1.0f);
                MusicItem musicItem = item.GetComponent<MusicItem>();
                if (musicItem != null)
                {
                    //用链表管理
                    GameObjNode _gameObjNode = this.gameObjNode;
                    while (_gameObjNode.next != null)
                    {
                        _gameObjNode = _gameObjNode.next;
                    }

                    _gameObjNode.next = new GameObjNode(item);
                    //item.transform.SetParent(_gameObjNode.go.transform);
                    musicItem.ItemBeEat(_gameObjNode.go);
                    playerData.GetMusicItem(musicItem.musicType);


                    int layer = LayerMask.NameToLayer(LayerData.beEat);
                    item.layer = layer;
                }
            }
        }
    }

    private void SetCameraPos()
    {
        Vector3 camPos = transform.position - transform.forward * playerData.camOffset.x +
                         transform.up * playerData.camOffset.y;
        //Debug.Log("player pos:" + camPos);
        GamePlayInfo.cameraPos = camPos;
    }

    private void UpdateCopyPlayerY()
    {
        Vector3 pos = this.gameObject.transform.position;
        if (-50.0f < pos.y && pos.y < 50.0f)
        {
            copyPlayer.transform.position = new Vector3(pos.x, pos.y + 100, pos.z);
        }
        else
        {
            copyPlayer.transform.position = new Vector3(pos.x, pos.y - 100, pos.z);
        }
    }

    private void UpdateCopyPlayerXZ()
    {
        Vector3 pos = this.gameObject.transform.position;
        Vector3 dir = this.gameObject.transform.forward;
        copyPlayer.transform.forward = dir;
        copyPlayer.transform.position =
            new Vector3(pos.x, copyPlayer.transform.position.y, pos.z);
    }

    #region 技能操作

    private float defaulSpeed;
    private float defaulMistyOut;
    private float defaulMistyIn;

    public void OnClickSaoMiao()
    {
        MusicManager.Instance.playSound(AudioData.saomiao);
        playerData.cursmSkillCD = playerData.maxSaoMiaoSkillCD;
        //StartCoroutine(ExecuteFunction());
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Touch");
        Transform near = gos[0].transform;
        float nearDistance = Vector3.Distance(near.position, transform.position);
        for (int i = 1; i < gos.Length; i++)
        {
            Transform t = gos[i].transform;
            float d = Vector3.Distance(t.position, transform.position);
            int layerId = gos[i].gameObject.layer;
            // 将层级ID转换为名称
            string layerName = LayerMask.LayerToName(layerId);
            if (d < nearDistance && layerName == LayerData.touch)
            {
                near = t;
                nearDistance = d;
            }
        }

        Material material = near.transform.GetComponent<MeshRenderer>().material;
        Color color = material.GetColor("_BaseColor");
        Shader.SetGlobalColor("_MusicItemColor", color);
        Debug.Log("new color is:" + color);
    }

    public void OnClickRun()
    {
        MusicManager.Instance.playSound(AudioData.touchInWater);
        playerData.curPlayFrameCount = 0;
        playerData.curRunSkillCD = playerData.maxRunSkillCD;
        // playerData.SetWorldId(0, true);
        // playerData.SetWorldId(1, true);
        //StartCoroutine(ExecuteFunction());
    }

    public void OnClickBackInit()
    {
        gameObjNode.next = null;
        transform.position = playerData.originalPos;
        transform.forward = playerData.originalDir;
    }

    /// <summary>
    /// 集中模式
    /// </summary>
    public void OnClickZhuanzhu()
    {
        playerData.speed = defaulSpeed * 0.7f;
        playerData.mistyOutRadius = defaulMistyOut - 1.5f;
        playerData.mistyInRadius = defaulMistyIn - 1.5f;

        int i = 0;
        //用链表管理
        GameObjNode _gameObjNode = this.gameObjNode;
        while (_gameObjNode != null)
        {
            i++;
            if (i > 10)
                break;
            MusicItem item = _gameObjNode.go.GetComponent<MusicItem>();
            _gameObjNode = _gameObjNode.next;
            if (item != null)
            {
                AudioSource source = item.Source;
                source.volume = 0.01f;
            }
        }
    }


    /// <summary>
    /// 集中模式恢复正常
    /// </summary>
    public void OnClickZhuanzhuBack()
    {
        playerData.speed = defaulSpeed;
        playerData.mistyOutRadius = defaulMistyOut;
        playerData.mistyInRadius = defaulMistyIn;

        int i = 0;
        //用链表管理
        GameObjNode _gameObjNode = this.gameObjNode;
        if (gameObjNode == null)
            return;
        while (_gameObjNode != null)
        {
            i++;
            if (i > 10)
                break;
            MusicItem item = _gameObjNode.go.GetComponent<MusicItem>();
            _gameObjNode = _gameObjNode.next;
            if (item != null)
            {
                AudioSource source = item.Source;
                source.volume = 0.4f;
            }
        }
    }


    IEnumerator ExecuteFunction()
    {
        int frameCount = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();

            frameCount++;

            if (frameCount > playerData.maxPlayFrameCount + 1)
            {
                break;
            }
        }

        if (playerData.currentWorld == 0)
        {
            playerData.SetWorldId(0, false);
            playerData.SetWorldId(1, true);
            playerData.currentWorld = 1;
        }
        else if(playerData.currentWorld == 1)
        {
            playerData.SetWorldId(1, false);
            playerData.SetWorldId(0, true);
            playerData.currentWorld = 0;
        }

        playerData.length = 0;
        Shader.SetGlobalFloat("_currentWorld",playerData.currentWorld);
        //在这里执行你的代码
        Vector3 pos = this.gameObject.transform.position;
        if (-50.0f < pos.y && pos.y < 50.0f)
        {
            transform.position = new Vector3(pos.x, pos.y + 100, pos.z);
            GameObjNode head = gameObjNode.next;
            while (head != null)
            {
                Vector3 _pos = head.go.transform.position;
                head.go.transform.position = new Vector3(_pos.x, _pos.y + 100, _pos.z);
                head = head.next;
            }
        }
        else
        {
            transform.position = new Vector3(pos.x, pos.y - 100, pos.z);
            GameObjNode head = gameObjNode.next;
            while (head != null)
            {
                Vector3 _pos = head.go.transform.position;
                head.go.transform.position = new Vector3(_pos.x, _pos.y - 100, _pos.z);
                head = head.next;
            }
        }

        //Shader.SetGlobalFloat("_TransitionLength", 0 * 8);
        // int frameCount = 0;
        // while (true)
        // {
        //     yield return new WaitForEndOfFrame();
        //
        //     frameCount++;
        //
        //     if (frameCount > 40)
        //     {
        //         playerData.curPlayFrameCount = 0;
        //         playerData.curRunSkillCD = playerData.maxRunSkillCD;
        //         
        //         yield break;
        //     }
        // }

        yield break;
    }

    public void ChangePlayerPos()
    {
        // playerData.length = 0;
        // Shader.SetGlobalFloat("_currentWorld",playerData.currentWorld);
        //在这里执行你的代码
        Vector3 pos = this.gameObject.transform.position;
        if (-50.0f < pos.y && pos.y < 50.0f)
        {
            transform.position = new Vector3(pos.x, pos.y + 100, pos.z);
            GameObjNode head = gameObjNode.next;
            while (head != null)
            {
                Vector3 _pos = head.go.transform.position;
                head.go.transform.position = new Vector3(_pos.x, _pos.y + 100, _pos.z);
                head = head.next;
            }
        }
        else
        {
            transform.position = new Vector3(pos.x, pos.y - 100, pos.z);
            GameObjNode head = gameObjNode.next;
            while (head != null)
            {
                Vector3 _pos = head.go.transform.position;
                head.go.transform.position = new Vector3(_pos.x, _pos.y - 100, _pos.z);
                head = head.next;
            }
        }

        Vector3 camPos = transform.position - transform.forward * playerData.camOffset.x +
                         transform.up * playerData.camOffset.y;
        GamePlayInfo.cameraPos = camPos;
        CameraManager.Instance.UpdateCameraPos(camPos);
    }

    #endregion

    #region 移动操作

    [Serializable]
    public enum MoveState
    {
        QianjingMove,
        HoutuiMove,
        None
    }

    public void OnPlayerMove(MoveState moveState, bool isMove = true)
    {
        if (moveState == MoveState.QianjingMove)
        {
            this.rb.velocity = this.rb.velocity.y * transform.up +
                               transform.forward * playerData.speed;
        }
        else if (moveState == MoveState.HoutuiMove)
        {
            this.rb.velocity = this.rb.velocity.y * transform.up -
                               transform.forward * playerData.speed;
        }
        else
        {
            this.rb.velocity = Vector3.Lerp(Vector3.zero, this.rb.velocity, playerData.stopSpeed);
        }
        //Debug.LogFormat("{0}--{1}--{2}",this.rb.velocity.y * transform.up,transform.forward * playerData.speed,rb.velocity);
        // Debug.Log(this.rb.velocity.y * transform.up);
        // Debug.Log(transform.forward * playerData.speed);
        // Debug.Log(this.rb.velocity);
        //Debug.Log(this.rb.velocity);
    }

    /// <summary>
    /// 旋转角色的接口
    /// </summary>
    /// <param name="d"></param>
    public void OnPlayerRotate(float d)
    {
        Vector3 dir = new Vector3(0, d * playerData.rotateSpeed, 0);
        this.transform.Rotate(dir);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);

        if (rot.eulerAngles.y > this.playerData.turnAngle && rot.eulerAngles.y < (360 - this.playerData.turnAngle))
        {
            rb.transform.forward = this.transform.forward;
            //this.SendEntityEvent(EntityEvent.None);
        }
    }

    #endregion
}
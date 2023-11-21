using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UIGamePlay : MonoSingleton<UIGamePlay>
{
    public Image smSkill_img;
    public Image runSkill_img;

    //收集了多少音符
    public Text progress_text;
    public Text second_text;
    public Text minute_text;

    [Header("测试用")]
    public TMP_InputField rotateSpeed;
    public TMP_InputField runSpeed;
    public TMP_Text touchCount;
    public TMP_Text fpsCount;


    protected override void OnStart()
    {
        _lastTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        FrameCalculate();

        if (rotateSpeed != null)
        {
            PlayerDataManager.Instance.rotateSpeed = int.Parse(rotateSpeed.text);
        }
            
        if (touchCount != null&&touchCount.enabled)
        {
            touchCount.text = Input.touchCount.ToString();
        }
        
        if (fpsCount != null&&fpsCount.enabled)
        {
            if ((int)Mathf.Floor(Time.time) >= currentMin)
            {
                currentMin = (int)Mathf.Floor(Time.time) + 1;
                string msg = string.Format("Fps:{0}", _Fps);
                fpsCount.text = msg;
            }
            
        }
    }

    #region MyRegion

    public int _frame = 0;
    private float _lastTime;
    private float _timeInterval;
    private float _Fps;
    private float _frameDeltaTime;
    
    private int currentMin = 0;
    private void FrameCalculate()
    {
        _frame++;
        if (Time.realtimeSinceStartup - _lastTime < _timeInterval)
        {
            return;
        }
 
        float time = Time.realtimeSinceStartup - _lastTime;
        _Fps = _frame / time;
        _frameDeltaTime = time / _frame;
 
        _lastTime = Time.realtimeSinceStartup;
        _frame = 0;
    }



    #endregion



    public void OnClickSaoMiao()
    {
        PlayerInputManager.Instance.OnClickSaoMiao();
    }
    
    public void OnClickRun()
    {
        PlayerInputManager.Instance.OnClickRun();
    }
    
    public void SetSmSkillCD(float cd)
    {
        smSkill_img.fillAmount = cd;
    }

    public void SetRunSkillCD(float cd)
    {
        runSkill_img.fillAmount = cd;
    }


    public void SetProgressText(float count)
    {
        progress_text.text = count.ToString();
    }

    public void SetTimeText(float count)
    {
        int second = (int)(count % 60);
        int minute = (int)(count / 60);
        second_text.text = (second).ToString(); 
        minute_text.text = (minute).ToString(); 
    }
}
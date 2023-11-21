using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerInputManager.MoveState moveState = PlayerInputManager.MoveState.QianjingMove;
    

    private void FixedUpdate()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Input.GetTouch(0).deltaPosition.x
        Debug.LogFormat("ui pos:{0}  touch pos:{1}",eventData.position.ToString(),Input.GetTouch(0).position.ToString());
        //Debug.LogFormat("");
        Debug.Log(moveState==PlayerInputManager.MoveState.QianjingMove?"Qianjing":"houtui");
        Debug.Log("OnPointerDown");
        // if (Vector2.Distance(eventData.position, Input.GetTouch(0).position) <= 0.01f&&Input.touchCount>=2)
        // {
        //     PlayerDataManager.Instance.curClickCount = 1;
        // }
        // else
        // {
        //     PlayerDataManager.Instance.curClickCount = 0;
        // }
        PlayerDataManager.Instance.playerMovePos = eventData.position;
        PlayerDataManager.Instance.moveState = moveState;
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(moveState==PlayerInputManager.MoveState.HoutuiMove?"Qianjing":"houtui");
        Debug.Log("OnPointerUp");
        PlayerDataManager.Instance.moveState = PlayerInputManager.MoveState.None;
    }
    
}
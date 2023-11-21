using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGamePlayRoate : MonoBehaviour//, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    private PlayerDataManager playerData;
    private Vector2 _MouseDownPos;

    private Vector2 _MouseCurrenPos;

    //用于验证点击
    private Vector2 _DragEventDataPos;

    private bool isDown = false;

    private void Awake()
    {
        playerData = PlayerDataManager.Instance;
    }

    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     _MouseDownPos = eventData.position;
    //     _MouseCurrenPos = eventData.position;
    //     playerData.curRotateVal = 0;
    //
    //     isDown = true;
    // }

    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     _MouseDownPos = eventData.position;
    //     _MouseCurrenPos = eventData.position;
    //     playerData.curRotateVal = 0;
    //
    //     isDown = false;
    // }

    // public void OnDrag(PointerEventData eventData)
    // {
    //     //Debug.LogFormat("OnBeginDrag:{0}", _MouseCurrenPos.x - _MouseDownPos.x);
    //     //_DragEventDataPos = eventData.position;
    //     //开始移动
    //
    //
    //     if (isDown)
    //     {
    //         _MouseDownPos = eventData.position;
    //         float rotateVal = (_MouseDownPos.x - _MouseCurrenPos.x) / Screen.width * 150;
    //
    //         if (Mathf.Abs(rotateVal) < 30)
    //         {
    //             playerData.curRotateVal = Mathf.Abs(rotateVal) > 0.000001f ? rotateVal : 0;
    //             Debug.LogFormat("start Rotate:{0}", rotateVal);
    //         }
    //         
    //         _MouseCurrenPos = eventData.position;
    //     }
    // }

    // private void FixedUpdate()
    // {
    //     Vector2 mousePos = Input.mousePosition;
    //     if (isDown&&Mathf.Abs(_DragEventDataPos.x-mousePos.x) / Screen.width * 150<10)
    //     {
    //         _MouseDownPos = mousePos;
    //         float rotateVal = (_MouseDownPos.x - _MouseCurrenPos.x) / Screen.width * 150;
    //         playerData.curRotateVal = Mathf.Abs(rotateVal) > 0.000001f ? rotateVal : 0;
    //         if (Mathf.Abs(rotateVal)>80)
    //             Debug.LogFormat("start Rotate:{0}", rotateVal);
    //
    //         _MouseCurrenPos = Input.mousePosition;
    //     }
    // }
}
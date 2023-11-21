using System;
using UnityEngine;

public class TransitionMgr : MonoBehaviour
{
    public PlayerDataManager playerData;
    public Color world1Color1 = Color.white;
    public Color world1Color2 = Color.white;

    private void Start()
    {
        //MyPlanarRef.Instance.CloseCamera();
    }

    private void LateUpdate()
    {
        if (playerData == null)
        {
            playerData = PlayerDataManager.Instance; 
        }
  
        
        if (playerData.curPlayFrameCount < playerData.maxPlayFrameCount)
        {
            MyPlanarRef.Instance.OpenCamera();
            playerData.curPlayFrameCount++;
            playerData.length =playerData.curPlayFrameCount / playerData.maxPlayFrameCount;
            Shader.SetGlobalFloat("_TransitionLength",  playerData.length * playerData.RunSpeed);
            GamePlayInfo.isOpenTransition = true;
        }
        else
        {
            //MyPlanarRef.Instance.CloseCamera();
            Shader.SetGlobalFloat("_TransitionLength", 
                0);
        }
        
        
        if (playerData.curPlayFrameCount >= playerData.maxPlayFrameCount&&playerData.length>=0.9f)
        {
            if (playerData.currentWorld == 0)
            {
                playerData.currentWorld = 1;
            }
            else if(playerData.currentWorld == 1)
            {
                playerData.currentWorld = 0;
            }

            playerData.length = 0;
            Shader.SetGlobalFloat("_currentWorld",playerData.currentWorld);
            Shader.SetGlobalFloat("_TransitionLength", 0);
            MyPlanarRef.Instance.CloseCamera();
            GamePlayInfo.isOpenTransition = false;
            PlayerInputManager.Instance.ChangePlayerPos();
        }
        
        
        
        Shader.SetGlobalColor("_RunWithColor",playerData.currentWorld==0?world1Color1:world1Color2);

        //Debug.Log("当前时间id是"+playerData.currentWorld);
        //Debug.Log("当前的距离"+playerData.length * playerData.RunSpeed);
    }
}
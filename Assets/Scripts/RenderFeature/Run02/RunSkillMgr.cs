using System;
using UnityEngine;


public class RunSkillMgr : MonoBehaviour
{
    public PlayerDataManager playerData;

    public Color world1Color1 = Color.white;
    public Color world1Color2 = Color.white;


    private void Awake()
    {
         
    }
    

    private void FixedUpdate()
    {
        if (playerData == null)
        {
            playerData = PlayerDataManager.Instance; 
        }


        if (playerData.curPlayFrameCount >= playerData.maxPlayFrameCount&&playerData.length>=0.9f)
        {
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
        }
        
        
        if (playerData.curPlayFrameCount < playerData.maxPlayFrameCount)
        {
            playerData.curPlayFrameCount++;
            playerData.length =playerData.curPlayFrameCount / playerData.maxPlayFrameCount;
            Shader.SetGlobalFloat("_TransitionLength",  playerData.length * playerData.RunSpeed);
        }
        else
        {
            Shader.SetGlobalFloat("_TransitionLength", 
                0);
        }
        
        
        
        Shader.SetGlobalColor("_RunWithColor",playerData.currentWorld==0?world1Color1:world1Color2);

        //Debug.Log("当前时间id是"+playerData.currentWorld);
        //Debug.Log("当前的距离"+playerData.length * playerData.RunSpeed);
    }
    
}
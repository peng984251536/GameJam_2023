using System;
using UnityEngine;


public class Item : MonoBehaviour
{
    private void Start()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 获取碰撞对象的层级ID
        int layerId = collision.gameObject.layer;

        // 将层级ID转换为名称
        string layerName = LayerMask.LayerToName(layerId);
        Debug.LogFormat("LayerName is {0}。goName is {1}", layerName, collision.gameObject.name);

        if (layerName == LayerData.player)
        {
            //TODO --播放音乐
            
            //MusicManager.Instance.playSound("saomiao");
        }
    }
}
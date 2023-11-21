using UnityEngine;

[ExecuteInEditMode]
public class MyObject : MonoBehaviour
{
    public int val;

    private void OnCollisionEnter(Collision collision)
    {
        // 判断碰撞的对象是否是玩家
        if (collision.gameObject.CompareTag("Player"))
        {
            // 执行你想要的操作
            Debug.Log("玩家碰撞到了这个物体！");
	
        }
    }
}
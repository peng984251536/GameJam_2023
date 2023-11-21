using System;
using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T) FindObjectOfType<T>();
            }

            return instance;
        }
    }

    //void Start()
    //{
    //    if (global)
    //    {
    //        if (instance != null && instance != this.gameObject.GetComponent<T>())
    //        {
    //            Destroy(this.gameObject);
    //            return;
    //        }
    //        DontDestroyOnLoad(this.gameObject);
    //        instance = this.gameObject.GetComponent<T>();//TODO
    //    }

    //    this.OnStart();
    //}

    /// <summary>
    /// AWAKE后的Start
    /// </summary>
    protected virtual void OnStart()
    {
    }

    protected virtual void OnAwake()
    {
    }

    void Awake()
    {
        //make sure we keep one instance of this script in the game
        Debug.LogWarningFormat("{0}[{1}] Awake", typeof(T), GetInstanceID());
        if (global)
        {
            if (instance != null && instance != gameObject.GetComponent<T>())
            {
                Destroy(gameObject);
                return;
            }

            // //搞不懂我之前写这个是为了啥
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            instance = this.gameObject.GetComponent<T>(); //TODO
        }

        this.OnAwake();
    }

    void Start()
    {
        this.OnStart();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private List<GameObject> ui_List = new List<GameObject>();
    private Dictionary<Type, GameObject> UIResources = new Dictionary<Type, GameObject>();

    UIManager()
    {
    }

    public T Show<T>()
    {
        Type type = typeof(T);
        GameObject UIRes;
        if (UIResources.TryGetValue(type, out UIRes))
        {
            UIRes.SetActive(true);
        }
        else
        {
            T t = default;
            for (int i = 0; i < ui_List.Count; i++)
            {
                GameObject go = ui_List[i].gameObject;
                t = go.GetComponent<T>();
                if (go.GetComponent<T>() != null)
                {
                    UIRes = Instantiate(go);
                    UIResources.Add(type, UIRes);
                    break;
                }
            }
        }

        return UIRes.GetComponent<T>();
    }

    public void Close(Type type)
    {
        GameObject UIRes;
        if (UIResources.TryGetValue(type, out UIRes))
        {
            UIRes.SetActive(false);
        }
    }
}
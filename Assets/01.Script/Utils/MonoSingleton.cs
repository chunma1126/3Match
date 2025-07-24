using UnityEngine;
using System;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;

    private static bool HasDontDestroyAttribute =>
        typeof(T).GetCustomAttributes(typeof(DontDestroyOnLoadAttribute), true).Length > 0;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject singletonObj = new GameObject(typeof(T).Name);
                    instance = singletonObj.AddComponent<T>();

                    if (HasDontDestroyAttribute)
                        DontDestroyOnLoad(singletonObj);
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;

            if (HasDontDestroyAttribute)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
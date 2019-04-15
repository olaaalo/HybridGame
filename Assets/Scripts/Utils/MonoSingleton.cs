using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T inst = null;

    public static T instance
    {
        get
        {
            inst = inst ?? (FindObjectOfType(typeof(T)) as T);
            inst = inst ?? new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
            return inst;
        }
    }

    private void OnApplicationQuit()
    {
        inst = null;
    }

    public static bool instanceExists
    {
        get
        {
            return inst != null;
        }
    }

    protected virtual void Awake()
    {
        inst = (T)this;
        DontDestroyOnLoad(this);
    }
}
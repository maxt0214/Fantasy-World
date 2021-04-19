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
                instance =(T)FindObjectOfType<T>();
            }
            return instance;
        }
    }

    void Start()
    {
        if (global) DontDestroyOnLoad(gameObject);
        OnStart();
    }

    protected virtual void OnStart()
    {

    }
}
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
                instance = FindObjectOfType<T>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (global)
        {
            if(instance != null && instance != gameObject.GetComponent<T>())
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = gameObject.GetComponent<T>();
        }
        OnStart();
    }

    protected virtual void OnStart()
    {

    }
}
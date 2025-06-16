using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                if (instance == null)
                {
                    Debug.LogError($"No instance of {typeof(T)} found in scene!");
                }
            }
            return instance;
        }
    }


    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this as T;
    }
}
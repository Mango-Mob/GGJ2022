using UnityEngine;

/*
 * Singleton by Tarodev
 * File: Singleton.cs
 * Description:
 *		An abstract class representation of a singleton class, which would be used as a class parent.
 *		This code was adopted from the video: https://www.youtube.com/watch?v=tE1qH8OxO2Y
 */
public abstract class Singleton<T> : MonoBehaviour where T :  MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/*
 * SingletonPersistent by Tarodev
 * File: Singleton.cs
 * Description:
 *		An abstract class representation of a singleton class, which would be used as a class parent.
 *		This version does not destroy itself when the scene is loaded.
 *		This code was adopted from the video: https://www.youtube.com/watch?v=tE1qH8OxO2Y
 */
public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        if(transform.parent != null)
            DontDestroyOnLoad(transform.parent);
        else
            DontDestroyOnLoad(gameObject);

        base.Awake();
    }
}

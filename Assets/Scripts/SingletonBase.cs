using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    public static T Instance => m_Instance;
    protected static T m_Instance;

    protected virtual void Awake()
    {
        if (m_Instance)
        {
            Debug.LogError("Can't have 2 instances of singleton type " + typeof(T).Name);
            Debug.LogWarning("Destroying duplicate instance " + name);
            Destroy(this);
        }

        m_Instance = this as T;
    }
}
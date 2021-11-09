using System;
using UnityEngine;
using System.Collections;

public class Poolable : MonoBehaviour
{
    public static event Action<Poolable> OnAnyPoolableDestroy;
    public event Action<Poolable> OnEnqueue;
    public event Action<Poolable> OnDequeue;

    public string key;
    public string Key
    {
        get
        {
            if (string.IsNullOrEmpty(key))
                key = gameObject.name;
            return key;
        }
        set => key = value;
    }

    public bool IsPooled { get; private set; }

    public void SetPooled(bool value, bool sendEvent)
    {
        IsPooled = value;
        if (!sendEvent) return;
        if(IsPooled) OnEnqueue?.Invoke(this);
        else OnDequeue?.Invoke(this);
    }
    
    private void OnDestroy()
    {
        OnAnyPoolableDestroy?.Invoke(this);
        //Debug.Log($"Destroyed {gameObject.name}");
    }
}
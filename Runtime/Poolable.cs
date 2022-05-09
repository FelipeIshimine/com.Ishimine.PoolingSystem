using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Poolable : MonoBehaviour
{
    public static event Action<Poolable> OnAnyPoolableDestroy;
    public event Action<Poolable> OnEnqueue;
    public event Action<Poolable> OnDequeue;

    private readonly Dictionary<Type, object> _components = new Dictionary<Type, object>();

    public bool IsPooled { get; private set; }

    public string key;
    public string Key
    {
        get
        {
            if (string.IsNullOrEmpty(key)) key = gameObject.name;
            return key;
        }
        set => key = value;
    }

    public void SetPooled(bool value, bool sendEvent)
    {
        IsPooled = value;
        if (!sendEvent) return;
        if(IsPooled) OnEnqueue?.Invoke(this);
        else OnDequeue?.Invoke(this);
    }
    
    public T Component<T>() where T : Component
    {
        var type = typeof(T);
        if (_components.TryGetValue(type, out object value))
            return (T)value;

        value = GetComponent<T>();
        T retValue = (T)value;
        SetComponent(retValue);
        return retValue;
    } 
    
    public void SetComponent<T>(T component) where T : Component => _components[typeof(T)] = component;
    
    private void OnDestroy()
    {
        OnAnyPoolableDestroy?.Invoke(this);
    }

}
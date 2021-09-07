using UnityEngine;
using System;
using System.Collections;

public abstract class BasePooler : MonoBehaviour
{
	#region Events
	public Action<Poolable> willEnqueue;
	public Action<Poolable> didDequeue;
	#endregion

	#region Fields / Properties
	public string key = string.Empty;
	public GameObject prefab = null;
	public int prepopulate = 0;
	public int maxCount = int.MaxValue;
	public bool autoRegister = true;
	
	public bool clearOnDestroy = false;
	public bool isRegistered { get; private set; }
	protected bool initialized = false;
            
	#endregion

	#region MonoBehaviour
	protected virtual void Awake ()
	{
        Initialize();
	}

	
    public void Initialize()
    {
        if (initialized) return;
        initialized = true;
        if (autoRegister)
            Register();
    }

    protected virtual void OnDestroy ()
	{
		//EnqueueAll();
		if (clearOnDestroy)
			UnRegister();
	}

  

    protected virtual void OnApplicationQuit()
	{
		EnqueueAll();
	}
	#endregion

	#region Public
	public void Register ()
	{
        if (string.IsNullOrEmpty(key))
        {
            key = prefab.name;
            //Debug.Log($"Register KEY: {key}");
        }
        //Debug.Log($"Register KEY: {key} Prepopulated:{prepopulate}");
        GameObjectPoolController.AddEntry(key, prefab, prepopulate, maxCount);
		isRegistered = true;
	}

	public void UnRegister ()
	{
		GameObjectPoolController.ClearEntry(key);
		isRegistered = false;
	}

	public virtual void Enqueue (Poolable item)
	{
        willEnqueue?.Invoke(item);
		GameObjectPoolController.Enqueue(item);
	}

	public virtual void EnqueueObject (GameObject obj)
	{
		Poolable item = obj.GetComponent<Poolable>();
		if (item != null) Enqueue (item);
	}

	public virtual void EnqueueScript (MonoBehaviour script)
	{
		Poolable item = script.GetComponent<Poolable>();
		if (item != null)
			Enqueue (item);
	}

	public virtual Poolable Dequeue ()
	{
		Poolable item = GameObjectPoolController.Dequeue(key);
        didDequeue?.Invoke(item);
		return item;
	}

    public virtual Component DequeueScript(Type type)
    {
        Poolable item = Dequeue();
        return item.GetComponent(type);
    }

    public virtual U DequeueScript<U> () where U : MonoBehaviour
	{
		Poolable item = Dequeue();
        return item.GetComponent<U>();
	}

	public abstract void EnqueueAll ();
	#endregion
}
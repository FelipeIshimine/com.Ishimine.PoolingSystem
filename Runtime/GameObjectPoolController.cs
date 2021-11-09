using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class GameObjectPoolController : MonoBehaviour 
{
	#region Fields / Properties
	static GameObjectPoolController Instance
	{
		get
		{
            if(_instance == null)
                CreateSharedInstance();
            return _instance;
		}
	}
	private static GameObjectPoolController _instance;
	
	[ShowInInspector]static Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();
	#endregion

	private Dictionary<string, Transform> _poolParent = new Dictionary<string, Transform>();
	
	#region MonoBehaviour


	void Awake ()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
		
		Poolable.OnAnyPoolableDestroy += OnAnyPoolableDestroy;

	}

	private void OnDestroy()
	{
		Poolable.OnAnyPoolableDestroy -= OnAnyPoolableDestroy;
	}

	private void OnAnyPoolableDestroy(Poolable obj)
	{
		//Debug.Log(obj.key);
		
		if (pools.ContainsKey(obj.key) && pools[obj.key].pool.Contains(obj))
		{
			List<Poolable> list = new List<Poolable>(pools[obj.key].pool);

			for (int i = 0; i < list.Count; i++)
			{
				if(list[i] == null || list[i] == obj)
					list.RemoveAt(i);
			}

			pools[obj.key].pool = new Queue<Poolable>(list);
		}
		
	}

	#endregion
	
	#region Public
	public static void SetMaxCount (string key, int maxCount)
	{
		if (!pools.ContainsKey(key))
			return;
		PoolData data = pools[key];
		data.maxCount = maxCount;
	}

	public static bool AddEntry (string key, GameObject prefab, int prepopulate, int maxCount)
	{
		if (pools.ContainsKey(key))
			return false;
		
		PoolData data = new PoolData();
		data.prefab = prefab;
		data.maxCount = maxCount;
		data.pool = new Queue<Poolable>(prepopulate);
		pools.Add(key, data);
        Debug.Log("<color=cyan> Added Entry:" + key + "</color>");

        for (int i = 0; i < prepopulate; ++i)
			EnqueueNewInstance(key, prefab);
		
		return true;
	}

	private static void EnqueueNewInstance(string key, GameObject prefab)=>Enqueue(CreateInstance(key, prefab), false);

	public static void ClearEntry (string key)
	{
		if (!pools.ContainsKey(key))
			return;
		
		PoolData data = pools[key];
		while (data.pool.Count > 0)
		{
			Poolable obj = data.pool.Dequeue();
			if (obj != null)
				GameObject.Destroy(obj.gameObject);
		}
        //Debug.Log($"Removed Key: {key}");
		pools.Remove(key);
	}

	public static void Enqueue(Poolable sender) => Enqueue(sender, true);

	private static void Enqueue (Poolable sender, bool sendEvent)
	{
		if (sender == null || sender.IsPooled || !pools.ContainsKey(sender.Key))
			return;
		
		PoolData data = pools[sender.Key];
		if (data.pool.Count >= data.maxCount)
		{
			GameObject.Destroy(sender.gameObject);
			Debug.LogWarning("Destroyed from pool");
			return;
		}
		
		data.pool.Enqueue(sender);
		sender.SetPooled(true, sendEvent);

		if (!Instance._poolParent.ContainsKey(sender.key))
		{
			Transform nRoot = new GameObject().transform;
			nRoot.gameObject.name = $"Pool:{sender.key}";
			nRoot.SetParent(Instance.transform);
			Instance._poolParent.Add(sender.key, nRoot);
		}
		sender.transform.SetParent(Instance._poolParent[sender.key]);
		UpdatePoolParentCount(sender.key);
		sender.gameObject.SetActive(false);
	}

	public static Poolable Dequeue (string key)
	{
        if (!pools.ContainsKey(key))
        {
            foreach (string item in pools.Keys)
                Debug.Log("KEY: " + item);

            return null;
        }

		PoolData data = pools[key];
		if (data.pool.Count == 0)
			EnqueueNewInstance(key, data.prefab);
		
		UpdatePoolParentCount(key);
		Poolable obj = data.pool.Dequeue();
		obj.SetPooled(false,true);
		obj.transform.SetParent(null);
		return obj;
	}

	private static void UpdatePoolParentCount(string key)
	{
		Instance._poolParent[key].name = $"Pool:{key} [{Instance._poolParent[key].childCount}]";
	}

	#endregion
	
	#region Private
	static void CreateSharedInstance ()
	{
		GameObject obj = new GameObject("GameObject Pool Controller");
		DontDestroyOnLoad(obj);
		_instance = obj.AddComponent<GameObjectPoolController>();
	}
	
	static Poolable CreateInstance (string key, GameObject prefab)
	{
        GameObject instance = Instantiate(prefab) as GameObject;
        //DontDestroyOnLoad(instance);
        Poolable p = instance.GetComponent<Poolable>();
        if (p == null) p = instance.AddComponent<Poolable>();
        
        p.Key = key;
		return p;
    }
	#endregion

	public static Poolable Dequeue(Poolable poolableParticlePrefab)
	{
		if (!pools.ContainsKey(poolableParticlePrefab.Key))
			AddEntry(poolableParticlePrefab.Key, poolableParticlePrefab.gameObject, 1, 100);
		return Dequeue(poolableParticlePrefab.Key);
	}
}
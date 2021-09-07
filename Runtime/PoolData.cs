using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class PoolData
{
	public GameObject prefab;
	public int maxCount;
	[ShowInInspector] public Queue<Poolable> pool;
}
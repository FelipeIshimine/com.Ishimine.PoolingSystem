using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling.Poolers
{
	public class SetPooler : BasePooler, IPool
	{
        public bool disableWhenEnqueue = true;
		#region Fields / Properties
		public HashSet<Poolable> collection = new HashSet<Poolable>();
		#endregion


		private void OnValidate()
		{
			key = prefab.name;
		}

		#region Public

		public SetPooler SetPrefab(GameObject nPrefab)
		{
			prefab = nPrefab;
			return this;
		}

		public override void Enqueue (Poolable item)
		{
			base.Enqueue(item);
			if (collection.Contains(item))
				collection.Remove(item);
			else
                Debug.LogWarning($"Doesnt contains item {item.gameObject.name} key:{item.key}");

            if (disableWhenEnqueue)
                item.gameObject.SetActive(false);
		}

		public override Poolable Dequeue ()
		{
			if(!initialized)
				Initialize();
			Poolable item = base.Dequeue();
			collection.Add(item);
			return item;
		}
		
		public Poolable DequeueAt(Transform parent, bool setActive = true)
		{
			var obj = Dequeue();
			obj.transform.SetParent(parent);
			if(setActive) obj.gameObject.SetActive(true);
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;
			return obj;
		}
		

        [Button]
		public override void EnqueueAll ()
		{
			foreach (Poolable item in collection)
				base.Enqueue(item);
			collection.Clear();
		}

        internal void Remove(Poolable poolable)
        {
            if (collection.Contains(poolable))
                collection.Remove(poolable);
        }

        #endregion

        
	}
}

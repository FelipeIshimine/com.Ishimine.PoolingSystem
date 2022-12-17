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

        public HashSet<Poolable> Collection { get; private set; } = new HashSet<Poolable>();

        #endregion


		protected override void OnValidate()
		{
			base.OnValidate();
			key = prefab ? prefab.name : string.Empty;
		}

		#region Public

		public SetPooler SetPrefab(GameObject nPrefab)
		{
			prefab = nPrefab;
			return this;
		}

		public override void Enqueue(Poolable item)
		{
			base.Enqueue(item);
			if (Collection.Contains(item))
				Collection.Remove(item);
			else
                Debug.LogWarning($"Doesnt contains item {item.gameObject.name} key:{item.key}");

            if (disableWhenEnqueue)
                item.gameObject.SetActive(false);
		}

		public override Poolable Dequeue()
		{
			if(!initialized)
				Initialize();
			Poolable item = base.Dequeue();
			Collection.Add(item);
			return item;
		}

		public T Dequeue<T>() where T : Component => Dequeue().Component<T>();

		public T DequeueAt<T>(Transform parent, bool setActive = true) where T : Component =>
			DequeueAt(parent, setActive).Component<T>();
		public Poolable DequeueAt(Transform parent, bool setActive = true)
		{
			var obj = Dequeue();

			var objTransform = obj.transform;
			objTransform.SetParent(parent);
			if(setActive) obj.gameObject.SetActive(true);
			
			objTransform.localScale = Vector3.one;
			objTransform.localPosition = Vector3.zero;
			return obj;
		}
		

        [Button]
		public override void EnqueueAll ()
		{
			foreach (Poolable item in Collection)
				base.Enqueue(item);
			Collection.Clear();
		}

        internal void Remove(Poolable poolable)
        {
            if (Collection.Contains(poolable))
                Collection.Remove(poolable);
        }

        #endregion
        
        
        

        
	}
}

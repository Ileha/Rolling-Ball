using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
	public interface IObjectPool {
		GameObject gameObject { get; }

		void OnCreate();
		void OnDestroy();
	}

	public class ObjectPool
	{
		private static Dictionary<IObjectPool, ObjectPool> availablePools = new Dictionary<IObjectPool, ObjectPool>();

		private Queue<IObjectPool> queue = new Queue<IObjectPool>();
		private Func<IObjectPool> create;

		public ObjectPool(Func<IObjectPool> create) {
			this.create = create;
		}

		public T Get<T>(Vector3 position, Quaternion rotation, Transform parent) where T:IObjectPool {
			IObjectPool res = null;
			if (queue.Count == 0) {
				res = create();

			}
			else {
				res = queue.Dequeue();
			}
			availablePools.Add(res, this);

			res.gameObject.transform.position = position;
			res.gameObject.transform.rotation = rotation;
			res.gameObject.transform.SetParent(parent, false);

			res.gameObject.SetActive(true);

			res.OnCreate();

			return (T) res;
		}

		public static void ReturnToPool(IObjectPool obj) {
			ObjectPool pool = availablePools[obj];
			availablePools.Remove(obj);
			pool.queue.Enqueue(obj);
			obj.gameObject.SetActive(false);
			obj.OnDestroy();
		}
	}
}

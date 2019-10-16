using System;
using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

namespace Level
{
	[RequireComponent(typeof(ParticleSystem))]
	public class Particles : MonoBehaviour, IObjectPool
	{
		public float LifeTime
		{
			get
			{
				return particles.main.duration;
			}
		}

		private ParticleSystem particles;

		public IEnumerator ReturnToPool() {
			yield return new WaitForSeconds(LifeTime);
			ObjectPool.ReturnToPool(this);
		}

		void Awake()
		{
			particles = gameObject.GetComponent<ParticleSystem>();
		}

		public void OnCreate()
		{
			particles.Play();
		}

		public void OnDestroy()
		{
			particles.Stop();
		}
	}
}
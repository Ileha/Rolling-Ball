using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;
using GameData;

namespace Level
{
	[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
	public class Good : LeveAccess, IObjectPool
	{
		void Awake() {
			
		}

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}

		void OnTriggerEnter2D(Collider2D other) {
			if (other.gameObject == level.ball.gameObject) {
				Particles splash = Singleton.Instanse.GetOnGoodParticles(transform.position);
				level.StartCoroutine(splash.ReturnToPool());
				gameObject.SetActive(false);
				level.OnGoodCatch(transform);
			}
		}

		public void OnCreate() {}
		public void OnDestroy() {}
	}
}

using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

namespace Level
{
	[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
	public class Barrier : LeveAccess
	{
		public float barrierStep = 0.64f;
		public float width {
			get { return sprite.size.x; }
		}

		private SpriteRenderer sprite;
		public BoxCollider2D trigger { get; private set; }

		void Awake() {
			sprite = gameObject.GetComponent<SpriteRenderer>();
			trigger = gameObject.GetComponent<BoxCollider2D>();
			trigger.isTrigger = true;
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		public void Generate(float width) {
			gameObject.SetActive(true);
			//if (active == false) { return; }

			if (gameObject.activeSelf) {
				sprite.size = new Vector2(barrierStep*(int) Random.Range(0,width),sprite.size.y);
			}

			trigger.size = sprite.size;
	   	 	trigger.offset = transform.InverseTransformPoint(sprite.bounds.center);
		}

		public bool IsActive() { return gameObject.activeSelf; } 

		void OnTriggerEnter2D(Collider2D other) {
			level.EndGame(other.gameObject);
		}
	}
}
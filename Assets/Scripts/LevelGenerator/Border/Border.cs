using UnityEngine;
using GameData;
using Pool;
using Level;

namespace LevelGenerator.Borders
{
	[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
	public abstract class Border : MonoBehaviour, IObjectPool
	{
		public Barrier barrierLeft;
		public Barrier barrierRight;
		public Good good;

		public Rigidbody2D rigidBody {
			get;
			private set;
		}
		public float width {
			get {
				return sprite.size.x;
			}
			set {
				sprite.size = new Vector2(value, sprite.size.y);
				OnWidthChange();
			}
		}
		protected SpriteRenderer sprite;

		void Awake() {
			sprite = gameObject.GetComponent<SpriteRenderer>();
			rigidBody = gameObject.GetComponent<Rigidbody2D>();
		}

		public void CreateLeftBarrier() {
			barrierLeft.Generate(width / 2);
		}
		public bool IsLeftBarrierActive() { return barrierLeft.IsActive(); }

		public void CreateRightBarrier() {
			barrierRight.Generate(width / 2);
			barrierRight.transform.localPosition = new Vector2(width, barrierRight.transform.localPosition.y);
		}
		public bool IsRightBarrierActive() { return barrierRight.IsActive(); }

		public void SetGood() {
			good.gameObject.SetActive(true);

			float leftSide = 0;
			float rightSide = width - good.GetComponent<SpriteRenderer>().size.x;
			if (barrierLeft.IsActive()) { leftSide += barrierLeft.width; }
			if (barrierRight.IsActive()) { rightSide -= barrierRight.width; }

			good.transform.localPosition = new Vector3(
				Random.Range(leftSide, rightSide), 
				good.transform.localPosition.y
			);
		}

		public virtual void OnCreate() {}
		public virtual void OnDestroy() {
			barrierLeft.Hide();
			barrierRight.Hide();
			good.gameObject.SetActive(false);
		}
		public virtual void OnWidthChange() {}

	}
}
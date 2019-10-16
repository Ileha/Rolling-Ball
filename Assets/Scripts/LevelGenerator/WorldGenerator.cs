using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using LevelGenerator.BorderFactory;
using LevelGenerator.Borders;
using System;
using Pool;
using Level;

namespace LevelGenerator
{
	public class Row : IEnumerable<Border> {
		public LinkedList<Border> container { get; private set; }
		private IBorderFactory factory;

		public Row(IBorderFactory factory) {
			container = new LinkedList<Border>();
			this.factory = factory;
		}

		public Border Last { get { return container.Last.Value; } }
		public Border First { get { return container.First.Value; } }

		public Border GetBorder(float width, Vector3 pos, Transform parent) {
			return factory.GetBorder(width, pos, parent);
		}

		public IEnumerator<Border> GetEnumerator()
		{
			return container.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	public class WorldGenerator : LeveAccess
	{
		public const float TOP_DOWN_PROBABILITY = 0.5f;
		public const float GOOD_APPEARANCE_PROBABILITY = 0.2f;
		public const float ZERO_SPACE_PROBABILITY = 0.2f;
		public const float BARRIER_PROBABILITY = 0.5f;

		public float StartWidth = 10;
		public float StartSpeed = 10;

		public float meters { get; private set; }
		public float RealWidth { 
			get {
				return level.Difficulty(StartWidth);
			}
		}
		public float RealSpeed { 
			get {
				return level.Difficulty(StartSpeed);
			}
		}

		private Row up = new Row(new TopBorderFactory());
		private Row down = new Row(new BottomBorderFactory());
		private bool generate = false;

		void Awake() {
			level.OnStartGame += OnStartGame;
			level.OnEndGame += OnEndGame;
		}

		// Use this for initialization
		void Start() {
			FillDefault();
			CreateTrigger();
		}

		// Update is called once per frame
		void Update() {

		}

		void FixedUpdate() {
			if (!generate) { return; }

			AddNext(up, down);

			Vector3 delta = new Vector2(-RealSpeed, 0) * Time.fixedDeltaTime;

			UpdateRow(up, delta);
			UpdateRow(down, delta);

			meters += RealSpeed * Time.fixedDeltaTime;

			if (up.First.transform.position.x + up.First.width< -Singleton.Instanse.screen.x) {
                DestroyFirst(up);
			}
			if (down.First.transform.position.x + down.First.width< -Singleton.Instanse.screen.x) {
				DestroyFirst(down);
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			level.EndGame(other.gameObject);
		}

		private void OnStartGame() {
			meters = 0;

			generate = true;
		}
		private void OnEndGame(RunData result) {
			generate = false;

			foreach (Border b in up) {
				ObjectPool.ReturnToPool(b);
			}
			up.container.Clear();

			foreach (Border b in down) {
				ObjectPool.ReturnToPool(b);
			}
			down.container.Clear();
            FillDefault();
		}

		private void CreateTrigger() {
			PolygonCollider2D coll = gameObject.AddComponent<PolygonCollider2D>();

			coll.pathCount = 2;

			coll.SetPath(0, new Vector2[] {
				new Vector2(Singleton.Instanse.screen.x, -Singleton.Instanse.screen.y / 2f-20f),
				new Vector2(-Singleton.Instanse.screen.x, -Singleton.Instanse.screen.y / 2f-20f),
				new Vector2(-Singleton.Instanse.screen.x, -Singleton.Instanse.screen.y / 2f),
				new Vector2(Singleton.Instanse.screen.x, -Singleton.Instanse.screen.y / 2f)
			});
			coll.SetPath(1, new Vector2[] {
				new Vector2(Singleton.Instanse.screen.x, Singleton.Instanse.screen.y / 2f+20f),
				new Vector2(-Singleton.Instanse.screen.x, Singleton.Instanse.screen.y / 2f+20f),
				new Vector2(-Singleton.Instanse.screen.x, Singleton.Instanse.screen.y / 2f),
				new Vector2(Singleton.Instanse.screen.x, Singleton.Instanse.screen.y / 2f)
			});

			coll.isTrigger = true;
		}
		private void UpdateRow(Row row, Vector3 delta) {
			foreach (Border b in row) {
				b.rigidBody.MovePosition(b.transform.position + delta);
			}
		}
		private void FillDefault() {
			up.container.AddLast(up.GetBorder(
				Singleton.Instanse.screen.x*UnityEngine.Random.Range(1f, 2f),
				new Vector2(-Singleton.Instanse.screen.x / 2, Singleton.Instanse.screen.y / 2f - 1f),
				transform));
			down.container.AddLast(down.GetBorder(
				Singleton.Instanse.screen.x*UnityEngine.Random.Range(1f, 2f),
				new Vector2(-Singleton.Instanse.screen.x / 2, -Singleton.Instanse.screen.y / 2f + 0.5f),
				transform));
		}
		private void DestroyFirst(Row row) {
			Border b = row.First;
			row.container.RemoveFirst();
			ObjectPool.ReturnToPool(b);
		}
		private void AddNext(Row upRow, Row downRow) {
			float upEndX = upRow.Last.transform.position.x + upRow.Last.width;
			float downEndX = downRow.Last.transform.position.x + downRow.Last.width;

			bool addUp = upEndX < Singleton.Instanse.screen.x;
			bool addDown = downEndX < Singleton.Instanse.screen.x;

			if (addUp && addDown) {//both
				Row continueRow = Singleton.RandomBetween<Row>(upRow, downRow, TOP_DOWN_PROBABILITY);
				Border border = AddNextInLine(continueRow, RealWidth, RealWidth, 1f);
				if (Singleton.RandomBetween<bool>(true, false, BARRIER_PROBABILITY)) {
					border.CreateRightBarrier();
				}
				if (Singleton.RandomBetween(true, false, GOOD_APPEARANCE_PROBABILITY)) {
					border.SetGood();
				}
			}
			else if (addUp && !addDown) {//add up
				float space = (downEndX - upEndX);
				Border border = AddNextInLine(upRow, RealWidth, space, ZERO_SPACE_PROBABILITY);
				if (Singleton.RandomBetween<bool>(true, false, BARRIER_PROBABILITY)) {
					border.CreateRightBarrier();
				}
				if (!downRow.Last.IsRightBarrierActive()) {
					if (Singleton.RandomBetween<bool>(true, false, BARRIER_PROBABILITY)) {
						border.CreateLeftBarrier();
					}
				}
				if (Singleton.RandomBetween(true, false, GOOD_APPEARANCE_PROBABILITY)) {
					border.SetGood();
				}
			}
			else if (!addUp && addDown) {//add down
				float space = (upEndX - downEndX);
				Border border = AddNextInLine(downRow, RealWidth, space, ZERO_SPACE_PROBABILITY);
				if (Singleton.RandomBetween<bool>(true, false, BARRIER_PROBABILITY)) {//генерация правого барьера
					border.CreateRightBarrier();
				}
				if (!upRow.Last.IsRightBarrierActive()) {
					if (Singleton.RandomBetween<bool>(true, false, BARRIER_PROBABILITY)) {//генерация левого барьера если сверху барьера нет
						border.CreateLeftBarrier();
					}
				}
				if (Singleton.RandomBetween(true, false, GOOD_APPEARANCE_PROBABILITY)) {
					border.SetGood();
				}

			}
		}
		private Border AddNextInLine(Row row, float width, float space, float zeroSpaceProbability) {
			Vector2 position = row.Last.rigidBody.transform.position;
			position.x += row.Last.width + Singleton.RandomBetween<float>(0, space * UnityEngine.Random.Range(0.5f, 1f), zeroSpaceProbability);

			row.container.AddLast(row.GetBorder(
				width * UnityEngine.Random.Range(0.5f, 1f),
				position,
				transform));

			return row.Last;
		}
	}
}
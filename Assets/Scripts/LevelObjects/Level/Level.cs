using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelGenerator;
using GameData;
using UnityEngine.EventSystems;
using Pool;

namespace Level
{
	public enum LevelState {
		Play,
		Pause,
		Stop
	}
	public delegate void StartEvent();
	public delegate void StopEvent(RunData results);

	public class Level : MonoBehaviour
	{
		public BallControll ball;
		public WorldGenerator generator;
		public Canvas WorldCanvas;
		public int Score { get; private set; }
		public LevelState State { get; private set; }

		public event StartEvent OnStartGame;
		public event StopEvent OnEndGame;

		private float timeScale = 1;
		private PointerEventData pointData;
		private List<RaycastResult> raycastResult = new List<RaycastResult>();

		private float RunTime = 0;

		void Awake() {
			State = LevelState.Stop;
			pointData = new PointerEventData(EventSystem.current);
		}

		void Update() {
			if (State == LevelState.Play) {
				RunTime += Time.deltaTime;
			}
		}

		public float Difficulty(float startValue) {
			return startValue + RunTime / 100f;
		}

		public bool IsUI(Vector3 ScreenPosition)
		{
			pointData.position = ScreenPosition;
			raycastResult.Clear();
			EventSystem.current.RaycastAll(pointData, raycastResult);
			if (raycastResult.Count == 0) {
				return false;
			}

			return true;
		}

		private IEnumerator AnimateInfoText(Transform sender, float time, string text, Color color) {
			InfoText info = Singleton.Instanse.GetInfoText(sender.position, WorldCanvas.transform);
			info.Set(text, color);

			RectTransform infoTransform = info.transform as RectTransform;
			float halfWidth = infoTransform.rect.width / 2f;

			if (infoTransform.position.x > Singleton.Instanse.screen.x / 2f-halfWidth) {
				infoTransform.position = new Vector3(Singleton.Instanse.screen.x / 2f-halfWidth, infoTransform.position.y);
			}
			else if (infoTransform.position.x < -Singleton.Instanse.screen.x / 2f + halfWidth) {
				infoTransform.position = new Vector3(-Singleton.Instanse.screen.x / 2f+halfWidth, infoTransform.position.y);
			}

			float startTime = 0;

			Vector3 endPos = new Vector3(info.transform.position.x, 0);
			Color endColor = color;
			endColor.a = 0;

			while (startTime < time)
			{
				info.transform.position = Vector3.MoveTowards(info.transform.position, endPos, Time.deltaTime);

				info.text.color = Color.Lerp(color, endColor, startTime / time);

				yield return null;
				startTime += Time.deltaTime;
			}
			ObjectPool.ReturnToPool(info);
		}

		public void OnGoodCatch(Transform sender) {
			StartCoroutine(AnimateInfoText(sender, 2f, "+10", Color.green));
			Score += 10;
		}
		public void NearBarrierScore(float distance, Transform sender) {

			int add = (int) ((Singleton.Instanse.screen.x / 4f) / (distance + 0.01f));
			add++;

			Score += add;
			if (distance > 1) {
				StartCoroutine(AnimateInfoText(sender, 2f, string.Format("+{0}", add), Color.green));
			}
			else {
                StartCoroutine(AnimateInfoText(sender, 2f, string.Format("narrow! +{0}", add), Color.green));
			}
		}

		public void StartGame() {
			RunTime = 0;
			Debug.Log("StartGame");
			Score = 0;
			if (OnStartGame != null) { OnStartGame(); }
			State = LevelState.Play;
		}
		public void Pause() {
			Debug.Log("pause");
			timeScale = (timeScale + 1) % 2;
			Time.timeScale = timeScale;
			State = timeScale == 0 ? LevelState.Pause : LevelState.Play;
		}
		private IEnumerator awaitCoroutine(IEnumerator another) {
			yield return StartCoroutine(another);

			RunData result = Singleton.Instanse.AddRun(new RunData(Score, (int)generator.meters)); //save here

			Debug.Log("EndGame");
			if (OnEndGame != null) { OnEndGame(result); }
			State = LevelState.Stop;
		}
		public void EndGame(GameObject enterObject) {
			if (enterObject == ball.gameObject) {
				StartCoroutine(awaitCoroutine(ball.DestroyCycle()));

			}
		}
	}
}

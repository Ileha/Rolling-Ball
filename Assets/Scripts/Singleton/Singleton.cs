using UnityEngine;
using LevelGenerator.Borders;
using Pool;
using Level;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace GameData {
	public class Singleton {
		private static Singleton instanse;
		public static Singleton Instanse {
			get {
				if (instanse == null) {
					instanse = new Singleton();
				}
				return instanse;
			}
		}

		public Vector2 screen { get; private set; }
		public Border TopBorder { get; private set; }
		public Border BottomBorder { get; private set; }
		public SpriteRenderer barrier { get; private set; }
		public Sprite Pause { get; private set; }
		public Sprite Play { get; private set; }
		public Data data { get; private set; }

		private ObjectPool onEndEmit;
		private ObjectPool onGoodEmit;
		private BinaryFormatter formatter = new BinaryFormatter();
		private ObjectPool UIRunInformation;
		private ObjectPool InfoTexts;
	#if UNITY_STANDALONE
		private string path = "data.dat";
	#endif
	#if UNITY_ANDROID || UNITY_IOS
		private string path = Application.persistentDataPath + "/data.dat";
	#endif

		public Singleton() {
			Load();

			screen = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight)) - Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
			screen.Set(Mathf.Abs(screen.x), Mathf.Abs(screen.y));
			TopBorder = Resources.Load<Border>("prefub/TopBorder");
			BottomBorder = Resources.Load<Border>("prefub/BottomBorder");
			barrier = Resources.Load<SpriteRenderer>("prefub/triangle");

			Particles onEnd = Resources.Load<Particles>("prefub/OnStar");
			onEndEmit = new ObjectPool(() => UnityEngine.Object.Instantiate(onEnd));

			Particles onGood = Resources.Load<Particles>("prefub/OnGood");
			onGoodEmit = new ObjectPool(() => UnityEngine.Object.Instantiate(onGood));

			Pause = Resources.Load<Sprite>("images/pause");
			Play = Resources.Load<Sprite>("images/play");

			RunInfo uiInformation = Resources.Load<RunInfo>("prefub/UI/RunInfo");
			UIRunInformation = new ObjectPool(() => UnityEngine.Object.Instantiate(uiInformation));

			InfoText info = Resources.Load<InfoText>("prefub/UI/Info");
			InfoTexts = new ObjectPool(() => UnityEngine.Object.Instantiate(info));
		}

		public Particles GetOnEndParticles(Vector3 pos) {
			return onEndEmit.Get<Particles>(pos, Quaternion.identity, null);
		}
		public Particles GetOnGoodParticles(Vector3 pos) {
			return onGoodEmit.Get<Particles>(pos, Quaternion.identity, null);
		}
		public RunInfo GetRunInfo(Transform transform) {
			return UIRunInformation.Get<RunInfo>(Vector3.zero, Quaternion.identity, transform);
		}

		public RunData AddRun(RunData information) {
			data.AddToRuns(information);
			Save();
			return information;
		}

		public InfoText GetInfoText(Vector3 pos, Transform transform) {
			return InfoTexts.Get<InfoText>(pos, Quaternion.identity, transform);
		}

		private void Save() {
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
				formatter.Serialize(fs, data);
			}
		}
		private void Load() {
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
				try {
					data = (Data)formatter.Deserialize(fs);
				}
				catch (Exception err) {
					if (data == null) {
						data = new Data();
					}
				}
			}

		}

		public static T RandomBetween<T>(T o1, T o2, float probability = 0.5f) {
			if (UnityEngine.Random.value > probability) {
				return o2;
			}
			else {
				return o1;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;
using UnityEngine.UI;
using GameData;

[RequireComponent(typeof(Text))]
public class RunInfo : MonoBehaviour, IObjectPool {
	private Text text;

	public void OnCreate() {}
	public void OnDestroy() {}

	void Awake() {
		text = gameObject.GetComponent<Text>();
	}

	public void SetInfo(RunData data) {
		text.text = string.Format("Run {0}\tScore: {1}\n\tDistance: {2}", data.runNumber, data.score, data.distance);
	}
}

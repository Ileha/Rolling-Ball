using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pool;
using GameData;
using Level;

[RequireComponent(typeof(Text))]
public class InfoText : MonoBehaviour, IObjectPool
{
	public Text text { get; private set; }

	void Awake() {
		text = gameObject.GetComponent<Text>();
	}

	public void Set(string text, Color color) {
		this.text.text = text;
		this.text.color = color;
	}

	public void OnCreate() {}

	public void OnDestroy() {}
}

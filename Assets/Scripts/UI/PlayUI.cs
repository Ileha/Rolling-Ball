using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Level;
using GameData;
using Pool;

[RequireComponent(typeof(Button))]
public class PlayUI : UIManagerAccess {
	void Awake() {
		gameObject.GetComponent<Button>().onClick.AddListener(PlayHandle);
	}

	void Start() {
        //ShowHightScores();
	}

	private void PlayHandle() {
		manager.GameLevel.StartGame();
	}
}

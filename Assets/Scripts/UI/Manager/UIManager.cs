using System.Collections;
using System.Collections.Generic;
using GameData;
using Level;
using UnityEngine;

public class UIManager : LeveAccess {
	public Death DeathPanel;
	public PlayUI PlayPanel;

	public Level.Level GameLevel { 
		get { return level; }
	}

	void Awake() {
		level.OnStartGame += OnStartGame;
		level.OnEndGame += OnStopGame;
	}

	// Use this for initialization
	void Start () {
		SetStartState();
	}

	public void SetStartState() {
		PlayPanel.gameObject.SetActive(true);
		DeathPanel.gameObject.SetActive(false);
	}

	private void OnStartGame() {
		PlayPanel.gameObject.SetActive(false);
		DeathPanel.gameObject.SetActive(false);
	}

	private void OnStopGame(RunData result) {
		PlayPanel.gameObject.SetActive(false);

		DeathPanel.ShowLastScore(result);
		DeathPanel.ShowHightScores();
		DeathPanel.gameObject.SetActive(true);
	}
}

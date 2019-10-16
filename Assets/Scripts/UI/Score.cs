using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Level;
using GameData;

public class Score : LeveAccess {
	public Text score;
	public Button pause;

	void Awake()
	{
		pause.onClick.AddListener(() => {
				level.Pause();
				if (level.State == LevelState.Pause) {
					((Image)(pause.targetGraphic)).overrideSprite = Singleton.Instanse.Play;
				}
				else if (level.State == LevelState.Play) {
					((Image)(pause.targetGraphic)).overrideSprite = Singleton.Instanse.Pause;
				}
			}
		);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		score.text = string.Format("DISTANCE: {0}\nSCORE: {1}", (int) level.generator.meters, level.Score);
	}
}

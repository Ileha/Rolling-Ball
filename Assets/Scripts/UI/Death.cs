using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using Pool;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Death : UIManagerAccess, IPointerClickHandler {
	public RectTransform resultsContent;
	public Text LastScore;

	public void ShowHightScores()
	{
		ClearBoard();
		Data data = Singleton.Instanse.data;

		for (int i = 0; i < data.runs.Length; i++)
		{
			if (data.runs[i] == null) { break; }
			Singleton.Instanse.GetRunInfo(resultsContent).SetInfo(data.runs[i]);
		}
	}

	private void ClearBoard()
	{
		RunInfo[] rules = resultsContent.GetComponentsInChildren<RunInfo>();
		for (int i = 0; i < rules.Length; i++)
		{
			ObjectPool.ReturnToPool(rules[i]);
		}
	}

	public void ShowLastScore(RunData result)
	{
		int place = Singleton.Instanse.data.GetPlace(result);
		if (place > 0) {
			LastScore.color = Color.green;
			LastScore.text = string.Format("CONGRATULATIONS!!! PLACE: {2}\nDISTANCE: {0}\tSCORE: {1}", result.distance, result.score, place);
		}
		else {
			LastScore.color = Color.white;
			LastScore.text = string.Format("DISTANCE: {0}\nSCORE: {1}", result.distance, result.score);
		}	
	}

	public void OnPointerClick(PointerEventData eventData) {
		manager.SetStartState();
	}
}

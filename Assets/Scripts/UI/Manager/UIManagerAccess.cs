using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerAccess : MonoBehaviour {
	private UIManager _manager;
	protected UIManager manager {
		get {
			if (_manager == null) {
				_manager = FindObjectOfType<UIManager>();
			}
			return _manager;
		}	
	}
}

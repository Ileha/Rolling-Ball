using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
	public abstract class LeveAccess : MonoBehaviour
	{
		private Level _level;
		protected Level level
		{
			get
			{
				if (_level == null)
				{
					_level = FindObjectOfType<Level>();
				}
				return _level;
			}
		}

	}
}

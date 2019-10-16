using UnityEngine;
using GameData;
using LevelGenerator.Borders;
using Pool;

namespace LevelGenerator.BorderFactory
{
	public class BottomBorderFactory : IBorderFactory
	{
		ObjectPool pool = new ObjectPool(() => Object.Instantiate(Singleton.Instanse.BottomBorder));

		public Border GetBorder(float width, Vector3 pos, Transform parent)
		{
			//GameObject go = Object.Instantiate(Singleton.Instanse.border.gameObject, pos, Quaternion.identity, parent);
			BottomBorder result = pool.Get<BottomBorder>(pos, Quaternion.identity, parent);//Object.Instantiate(Singleton.Instanse.BottomBorder, pos, Quaternion.identity, parent) as BottomBorder;
			result.width = width;
			return result;
		}
	}
}

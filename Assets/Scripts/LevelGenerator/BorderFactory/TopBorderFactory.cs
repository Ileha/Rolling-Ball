using UnityEngine;
using GameData;
using LevelGenerator.Borders;
using Pool;

namespace LevelGenerator.BorderFactory
{
	public class TopBorderFactory : IBorderFactory
	{
		ObjectPool pool = new ObjectPool(() => Object.Instantiate(Singleton.Instanse.TopBorder));

		public Border GetBorder(float width, Vector3 pos, Transform parent)
		{
			//GameObject go = Object.Instantiate(Singleton.Instanse.border.gameObject, pos, Quaternion.identity, parent);
			TopBorder result = pool.Get<TopBorder>(pos, Quaternion.identity, parent);//Object.Instantiate(Singleton.Instanse.TopBorder, pos, Quaternion.identity, parent) as TopBorder;
			result.width = width;
			return result;
		}
	}
}

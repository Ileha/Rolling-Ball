using UnityEngine;
using LevelGenerator.Borders;

namespace LevelGenerator.BorderFactory
{
	public interface IBorderFactory
	{
		Border GetBorder(float width, Vector3 pos, Transform parent);
	}
}

using UnityEngine;
using GameData;
using Level;
using Pool;

namespace LevelGenerator.Borders
{
	[RequireComponent(typeof(EdgeCollider2D))]
	public class BottomBorder : Border
	{
		public override void OnWidthChange()
		{
			EdgeCollider2D edge = gameObject.GetComponent<EdgeCollider2D>();
			edge.points = new Vector2[] {
				new Vector2(0f,sprite.size.y/2f), new Vector2(width, sprite.size.y/2f)	
			};
		}
	}
}
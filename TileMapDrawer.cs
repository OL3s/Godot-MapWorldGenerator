using Godot;
using System.Collections.Generic;

public static class TileMapDrawer
{
	public static void DrawMapToTileMapLayer(TileMapLayer layer, int[,] map, Dictionary<int, Vector2I> atlas, bool drawNoneTiles)
	{
		layer.Clear();

		for(var x = 0; x < map.GetLength(0); x++)
		{
			for(var y = 0; y < map.GetLength(1); y++)
			{
				int tileType = map[x, y];
				if(!drawNoneTiles && tileType == 0)
					continue;

				layer.SetCell(new Vector2I(x, y), 0, atlas[tileType]);
			}
		}
	}
}

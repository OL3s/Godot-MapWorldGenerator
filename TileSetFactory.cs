using Godot;
using System.Collections.Generic;

public static class TileSetFactory
{
	public static TileSet GenerateHeightTileSet()
	{
		return GenerateTileSetFromAtlas("res://assets/layers/sprLayerHeight.png", DictionaryTileset.HeightMapAtlas);
	}

	public static TileSet GenerateResourceTileSet()
	{
		return GenerateTileSetFromAtlas("res://assets/layers/sprLayerResource.png", DictionaryTileset.ResourceMapAtlas);
	}

	private static TileSet GenerateTileSetFromAtlas(string texturePath, Dictionary<int, Vector2I> atlas)
	{
		TileSet tileSet = new TileSet();
		tileSet.TileSize = new Vector2I(8, 8);
		Texture2D texture = GD.Load<Texture2D>(texturePath);
		if(texture == null)
		{
			GD.PushError("Could not load tileset texture: " + texturePath);
			return tileSet;
		}

		TileSetAtlasSource atlasSource = new TileSetAtlasSource();
		atlasSource.Texture = texture;
		atlasSource.TextureRegionSize = new Vector2I(8, 8);

		foreach(Vector2I atlasCoords in atlas.Values)
			atlasSource.CreateTile(atlasCoords);

		tileSet.AddSource(atlasSource, 0);
		return tileSet;
	}
}

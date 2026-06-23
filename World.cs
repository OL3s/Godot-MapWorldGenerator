using Godot;

public partial class World : Node2D
{
	private TileMapLayer heightLayer;
	private TileMapLayer resourceLayer;

	public override void _Ready()
	{
		BuildTileMapLayers();
		GenerateMap(64, 64);
	}

	public void GenerateMap(int width, int height)
	{
		ClearMap();

		float[,] floorNoiseMap = NoiseGenerator.GenerateIslandNoiseMap(width, height);
		float[,] foliageMap = NoiseGenerator.GenerateNoiseMap(width, height, -1.0f, 1.0f);
		float[,] mineralMap = NoiseGenerator.GenerateNoiseMap(width, height, -1.0f, 1.0f);
		int[,] heightMap = FloorHeightGenerator.GenerateHeightMap(floorNoiseMap);
		int[,] resourceMap = ResourceGenerator.GenerateResourceMap(heightMap, foliageMap, mineralMap);

		TileMapDrawer.DrawMapToTileMapLayer(heightLayer, heightMap, DictionaryTileset.HeightMapAtlas, true);
		TileMapDrawer.DrawMapToTileMapLayer(resourceLayer, resourceMap, DictionaryTileset.ResourceMapAtlas, false);
	}

	public void ClearMap()
	{
		heightLayer?.Clear();
		resourceLayer?.Clear();
	}

	private void BuildTileMapLayers()
	{
		heightLayer = GetNodeOrNull<TileMapLayer>("HeightLayer") ?? new TileMapLayer();
		resourceLayer = GetNodeOrNull<TileMapLayer>("ResourceLayer") ?? new TileMapLayer();

		heightLayer.Name = "HeightLayer";
		resourceLayer.Name = "ResourceLayer";

		heightLayer.TileSet = TileSetFactory.GenerateHeightTileSet();
		resourceLayer.TileSet = TileSetFactory.GenerateResourceTileSet();

		if(heightLayer.GetParent() == null)
			AddChild(heightLayer);
		if(resourceLayer.GetParent() == null)
			AddChild(resourceLayer);
	}
}

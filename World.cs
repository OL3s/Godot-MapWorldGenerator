using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public partial class World : Node2D
{
	TileMapLayer heightLayer;
	TileMapLayer resourceLayer;


	// === GODOT CALLBACKS ===
	public override void _Ready()
	{
		buildTileMapLayer();
		GenerateMap(64, 64);
	}

	public void GenerateMap(int width, int height)
	{
		ClearMap();

		float[,] noiseMap = generateNoiceIsland(width, height);
		int[,] heightMap = generateHeightMap(noiseMap);
		int[,] resourceMap = generateResourceMap(heightMap);

		drawMapToTileMapLayer(heightLayer, heightMap, DictionaryTileset.HeightMapAtlas, true);
		drawMapToTileMapLayer(resourceLayer, resourceMap, DictionaryTileset.ResourceMapAtlas, false);
	}

	public void ClearMap()
	{
		heightLayer?.Clear();
		resourceLayer?.Clear();
	}

	private void buildTileMapLayer()
	{
		heightLayer = GetNodeOrNull<TileMapLayer>("HeightLayer") ?? new TileMapLayer();
		resourceLayer = GetNodeOrNull<TileMapLayer>("ResourceLayer") ?? new TileMapLayer();

		heightLayer.Name = "HeightLayer";
		resourceLayer.Name = "ResourceLayer";

		heightLayer.TileSet = generateTileSetForHeightLayer();
		resourceLayer.TileSet = generateTileSetForResourceLayer();

		if(heightLayer.GetParent() == null)
			AddChild(heightLayer);
		if(resourceLayer.GetParent() == null)
			AddChild(resourceLayer);
	}
	private TileSet generateTileSetForHeightLayer()
	{
		// 8x8 resolution, 4x4 tileset
		return generateTileSetFromAtlas("res://assets/layers/sprLayerHeight.png", DictionaryTileset.HeightMapAtlas);
	}

	private TileSet generateTileSetForResourceLayer()
	{
		// 8x8 resolution, 4x4 tileset
		return generateTileSetFromAtlas("res://assets/layers/sprLayerResource.png", DictionaryTileset.ResourceMapAtlas);
	}

	private TileSet generateTileSetFromAtlas(string texturePath, Dictionary<int, Vector2I> atlas)
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


	// === BACKEND GENERATE MAP ===
	private float[,] generateNoiceMap(int width, int height, float min = 0.0f, float max = 1.0f, int smoothRadius = 3, int smoothIterations = 4)
	{
		float[,] map = new float[width, height];
		for(var x = 0; x < width; x++)
			for(var y = 0; y < height; y++)
				map[x, y] = (float)Random.Shared.NextDouble() * (max - min) + min;

		for(var i = 0; i <= smoothIterations; i++)
			map = averageNoiseMap(map, smoothRadius);

		normalizeNoiseMap(map, min, max);

		return map;
	}

	private float[,] generateNoiceIsland(int width, int height, float min = 0.0f, float max = 1.0f, int edgeDivisor = 8)
	{
		float[,] randomMap = generateNoiceMap(width, height, 0.0f, 1.0f);
		float[,] islandMap = new float[width, height];
		float averageMapSize = (width + height) / 2.0f;
		float edgeSize = Math.Max(1.0f, averageMapSize / edgeDivisor);
		float centerX = (width - 1) / 2.0f;
		float centerY = (height - 1) / 2.0f;
		float radiusX = width / 2.0f;
		float radiusY = height / 2.0f;
		float averageRadius = (radiusX + radiusY) / 2.0f;
		float edgeSizeNormalized = Math.Clamp(edgeSize / averageRadius, 0.0f, 1.0f);

		for(var x = 0; x < width; x++)
		{
			for(var y = 0; y < height; y++)
			{
				float normalizedX = (x - centerX) / radiusX;
				float normalizedY = (y - centerY) / radiusY;
				float distanceFromCenter = (float)Math.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
				float distanceToIslandEdge = 1.0f - distanceFromCenter;
				float edgeStrength = Math.Clamp(distanceToIslandEdge / edgeSizeNormalized, 0.0f, 1.0f);
				edgeStrength = edgeStrength * edgeStrength * (3.0f - 2.0f * edgeStrength);

				float islandValue = randomMap[x, y] * edgeStrength;
				islandValue = Math.Clamp(islandValue, 0.0f, 1.0f);
				islandMap[x, y] = islandValue * (max - min) + min;
			}
		}

		return islandMap;
	}

	private float[,] averageNoiseMap(float[,] noiseMap, int radius)
	{
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);
		float[,] averagedMap = new float[width, height];

		for(var x = 0; x < width; x++)
		{
			for(var y = 0; y < height; y++)
			{
				float total = 0.0f;
				int count = 0;
				int minX = Math.Max(0, x - radius);
				int maxX = Math.Min(width - 1, x + radius);
				int minY = Math.Max(0, y - radius);
				int maxY = Math.Min(height - 1, y + radius);

				for(var sampleX = minX; sampleX <= maxX; sampleX++)
				{
					for(var sampleY = minY; sampleY <= maxY; sampleY++)
					{
						total += noiseMap[sampleX, sampleY];
						count++;
					}
				}

				averagedMap[x, y] = total / count;
			}
		}

		return averagedMap;
	}

	private void normalizeNoiseMap(float[,] noiseMap, float min, float max)
	{
		float currentMin = float.MaxValue;
		float currentMax = float.MinValue;

		for(var x = 0; x < noiseMap.GetLength(0); x++)
		{
			for(var y = 0; y < noiseMap.GetLength(1); y++)
			{
				currentMin = Math.Min(currentMin, noiseMap[x, y]);
				currentMax = Math.Max(currentMax, noiseMap[x, y]);
			}
		}

		float currentRange = currentMax - currentMin;
		if(currentRange == 0.0f)
			return;

		for(var x = 0; x < noiseMap.GetLength(0); x++)
		{
			for(var y = 0; y < noiseMap.GetLength(1); y++)
			{
				float normalizedValue = (noiseMap[x, y] - currentMin) / currentRange;
				noiseMap[x, y] = normalizedValue * (max - min) + min;
			}
		}
	}

	public void printGenerationLog(int width, int height, float[,] noiseMap, int[,] heightMap, int[,] resourceMap)
	{
		int chunkSize = getGenerationLogChunkSize(width, height);
		StringBuilder output = new StringBuilder();

		output.AppendLine("==! GENERATION LOGGER !==");
		output.AppendLine();
		output.AppendLine("== DETAILS ==");
		output.AppendLine("Map size: " + width + "x" + height);
		output.AppendLine("Chunk size: " + chunkSize + "x" + chunkSize);
		output.AppendLine();

		appendFloatChunkAverages(output, noiseMap, "Noice Map Chunk Averages", chunkSize);
		appendIntGridSummary(output, heightMap, "Height Map Summary", typeof(DictionaryTileset.GenerateHeightType));
		appendIntGridSummary(output, resourceMap, "Resource Map Summary", typeof(DictionaryTileset.GenerateResourceType));

		GD.Print(output.ToString());
	}

	private int getGenerationLogChunkSize(int width, int height)
	{
		const int targetChunkCount = 16;
		return Math.Max(1, (int)Math.Ceiling(Math.Max(width, height) / (float)targetChunkCount));
	}

	private void appendFloatChunkAverages(StringBuilder output, float[,] grid, string header, int chunkSize)
	{
		output.AppendLine("== " + header + " ==");

		for(var chunkY = 0; chunkY < grid.GetLength(1); chunkY += chunkSize)
		{
			for(var chunkX = 0; chunkX < grid.GetLength(0); chunkX += chunkSize)
			{
				float total = 0.0f;
				int count = 0;
				int maxX = Math.Min(chunkX + chunkSize, grid.GetLength(0));
				int maxY = Math.Min(chunkY + chunkSize, grid.GetLength(1));

				for(var x = chunkX; x < maxX; x++)
				{
					for(var y = chunkY; y < maxY; y++)
					{
						total += grid[x, y];
						count++;
					}
				}

				output.Append((total / count).ToString("0.00"));
				output.Append(" ");
			}

			output.AppendLine();
		}
		output.AppendLine();
	}

	private void appendIntGridSummary(StringBuilder output, int[,] grid, string header, Type enumType)
	{
		Dictionary<int, int> counts = new Dictionary<int, int>();
		int totalTiles = grid.GetLength(0) * grid.GetLength(1);

		for(var x = 0; x < grid.GetLength(0); x++)
			for(var y = 0; y < grid.GetLength(1); y++)
			{
				int value = grid[x, y];
				if(!counts.ContainsKey(value))
					counts[value] = 0;

				counts[value]++;
			}

		output.AppendLine("== " + header + " ==");
		foreach(int value in Enum.GetValues(enumType))
		{
			int count = counts.GetValueOrDefault(value, 0);
			float percent = (float)count / totalTiles * 100.0f;
			output.AppendLine(Enum.GetName(enumType, value) + ": " + count + " (" + percent.ToString("0.0") + "%)");
		}

		output.AppendLine();
	}

	private int[,] generateHeightMap(float[,] noiseMap)
	{
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);
		int[,] heightMap = new int[width, height];

		for(var x = 0; x < width; x++)
		{
			for(var y = 0; y < height; y++)
			{
				float value = noiseMap[x, y];

				if(value < 0.22f)
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.DeepWater;
				else if(value < 0.34f)
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.ShallowWater;
				else if(value < 0.44f)
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.Shore;
				else if(value < 0.60f)
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.Lowland;
				else if(value < 0.72f)
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.Highland;
				else if(value < 0.86f)
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.Mountain;
				else
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.SnowPeak;
			}
		}

		return heightMap;
	}

	private int[,] generateResourceMap(int[,] heightMap)
	{
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		int[,] resourceMap = new int[width, height];

		for(var x = 0; x < width; x++)
		{
			for(var y = 0; y < height; y++)
				resourceMap[x, y] = generateResourceForHeight(heightMap[x, y]);
		}

		return resourceMap;
	}

	private int generateResourceForHeight(int heightType)
	{
		double roll = Random.Shared.NextDouble();

		switch((DictionaryTileset.GenerateHeightType)heightType)
		{
			case DictionaryTileset.GenerateHeightType.Shore:
				if(roll < 0.05f) return (int)DictionaryTileset.GenerateResourceType.Stone;
				if(roll < 0.10f) return (int)DictionaryTileset.GenerateResourceType.Bush;
				break;

			case DictionaryTileset.GenerateHeightType.Lowland:
				if(roll < 0.15f) return (int)DictionaryTileset.GenerateResourceType.Tree;
				if(roll < 0.35f) return (int)DictionaryTileset.GenerateResourceType.Bush;
				break;

			case DictionaryTileset.GenerateHeightType.Highland:
				if(roll < 0.20f) return (int)DictionaryTileset.GenerateResourceType.Tree;
				if(roll < 0.30f) return (int)DictionaryTileset.GenerateResourceType.Stone;
				break;

			case DictionaryTileset.GenerateHeightType.Mountain:
				if(roll < 0.20f) return (int)DictionaryTileset.GenerateResourceType.Stone;
				if(roll < 0.30f) return (int)DictionaryTileset.GenerateResourceType.Mineral;
				break;

			case DictionaryTileset.GenerateHeightType.SnowPeak:
				if(roll < 0.10f) return (int)DictionaryTileset.GenerateResourceType.Mineral;
				break;
		}

		return (int)DictionaryTileset.GenerateResourceType.None;
	}

	private void drawMapToTileMapLayer(TileMapLayer layer, int[,] map, Dictionary<int, Vector2I> atlas, bool drawNoneTiles)
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

	private TileMapLayer convertToTileMapLayer(int[,] grid)
	{
		return new TileMapLayer();
	}


}

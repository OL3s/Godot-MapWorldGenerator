public static class FloorHeightGenerator
{
	public static int[,] GenerateHeightMap(float[,] noiseMap)
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
				else if(value < 0.72f)
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.Land;
				else
					heightMap[x, y] = (int)DictionaryTileset.GenerateHeightType.Mountain;
			}
		}

		return heightMap;
	}
}

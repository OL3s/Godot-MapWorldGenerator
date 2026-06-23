using System;

public static class NoiseGenerator
{
	public static float[,] GenerateNoiseMap(int width, int height, float min = 0.0f, float max = 1.0f, int smoothRadius = 3, int smoothIterations = 4)
	{
		float[,] map = new float[width, height];
		for(var x = 0; x < width; x++)
			for(var y = 0; y < height; y++)
				map[x, y] = (float)Random.Shared.NextDouble() * (max - min) + min;

		for(var i = 0; i <= smoothIterations; i++)
			map = AverageNoiseMap(map, smoothRadius);

		NormalizeNoiseMap(map, min, max);
		return map;
	}

	public static float[,] GenerateIslandNoiseMap(int width, int height, float min = 0.0f, float max = 1.0f, int edgeDivisor = 8)
	{
		float[,] randomMap = GenerateNoiseMap(width, height, 0.0f, 1.0f);
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

	private static float[,] AverageNoiseMap(float[,] noiseMap, int radius)
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

	private static void NormalizeNoiseMap(float[,] noiseMap, float min, float max)
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
			for(var y = 0; y < noiseMap.GetLength(1); y++)
			{
				float normalizedValue = (noiseMap[x, y] - currentMin) / currentRange;
				noiseMap[x, y] = normalizedValue * (max - min) + min;
			}
	}
}

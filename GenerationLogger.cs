using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public static class GenerationLogger
{
	public static void PrintGenerationLog(int width, int height, float[,] noiseMap, int[,] heightMap, int[,] resourceMap)
	{
		GD.Print(BuildGenerationLog(BuildGenerationStats(width, height, noiseMap, heightMap, resourceMap)));
	}

	public static string BuildGenerationLog(int width, int height, float[,] noiseMap, int[,] heightMap, int[,] resourceMap)
	{
		return BuildGenerationLog(BuildGenerationStats(width, height, noiseMap, heightMap, resourceMap));
	}

	public static GenerationStats BuildGenerationStats(int width, int height, float[,] noiseMap, int[,] heightMap, int[,] resourceMap)
	{
		GenerationStats stats = new GenerationStats();
		stats.Width = width;
		stats.Height = height;
		stats.ChunkSize = GetGenerationLogChunkSize(width, height);

		AppendFloatChunkAverages(stats, noiseMap, stats.ChunkSize);
		AppendIntGridSummary(stats.HeightStats, heightMap, typeof(DictionaryTileset.GenerateHeightType));
		AppendIntGridSummary(stats.ResourceStats, resourceMap, typeof(DictionaryTileset.GenerateResourceType));

		return stats;
	}

	public static string BuildGenerationLog(GenerationStats stats)
	{
		StringBuilder output = new StringBuilder();

		output.AppendLine("==! GENERATION LOGGER !==");
		output.AppendLine();
		output.AppendLine("== DETAILS ==");
		output.AppendLine("Map size: " + stats.Width + "x" + stats.Height);
		output.AppendLine("Chunk size: " + stats.ChunkSize + "x" + stats.ChunkSize);
		output.AppendLine();

		AppendFloatChunkAveragesLog(output, stats, "Noise Map Chunk Averages");
		AppendIntGridSummaryLog(output, stats.HeightStats, "Height Map Summary");
		AppendIntGridSummaryLog(output, stats.ResourceStats, "Resource Map Summary");

		return output.ToString();
	}

	private static int GetGenerationLogChunkSize(int width, int height)
	{
		const int targetChunkCount = 16;
		return Math.Max(1, (int)Math.Ceiling(Math.Max(width, height) / (float)targetChunkCount));
	}

	private static void AppendFloatChunkAverages(GenerationStats stats, float[,] grid, int chunkSize)
	{
		for(var chunkY = 0; chunkY < grid.GetLength(1); chunkY += chunkSize)
		{
			int currentChunkColumns = 0;
			for(var chunkX = 0; chunkX < grid.GetLength(0); chunkX += chunkSize)
			{
				float total = 0.0f;
				int count = 0;
				int maxX = Math.Min(chunkX + chunkSize, grid.GetLength(0));
				int maxY = Math.Min(chunkY + chunkSize, grid.GetLength(1));

				for(var x = chunkX; x < maxX; x++)
					for(var y = chunkY; y < maxY; y++)
					{
						total += grid[x, y];
						count++;
					}

				float average = total / count;
				stats.NoiseChunkAverages.Add(average);
				currentChunkColumns++;
			}

			stats.NoiseColumns = Math.Max(stats.NoiseColumns, currentChunkColumns);
		}
	}

	private static void AppendFloatChunkAveragesLog(StringBuilder output, GenerationStats stats, string header)
	{
		output.AppendLine("== " + header + " ==");
		for(var i = 0; i < stats.NoiseChunkAverages.Count; i++)
		{
			output.Append(stats.NoiseChunkAverages[i].ToString("0.00"));
			output.Append(" ");
			if((i + 1) % stats.NoiseColumns == 0)
				output.AppendLine();
		}
		output.AppendLine();

		output.AppendLine("== " + header + " Visual ==");
		for(var i = 0; i < stats.NoiseChunkAverages.Count; i++)
		{
			output.Append(GetFillChar(stats.NoiseChunkAverages[i]));
			if((i + 1) % stats.NoiseColumns == 0)
				output.AppendLine();
		}

		output.AppendLine();
	}

	private static char GetFillChar(float value)
	{
		const string fillChars = " ░▒▓█";
		int index = Mathf.Clamp((int)Math.Round(Math.Clamp(value, 0.0f, 1.0f) * (fillChars.Length - 1)), 0, fillChars.Length - 1);
		return fillChars[index];
	}

	private static void AppendIntGridSummary(List<GenerationStatEntry> entries, int[,] grid, Type enumType)
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

		foreach(int value in Enum.GetValues(enumType))
		{
			int count = counts.GetValueOrDefault(value, 0);
			float percent = (float)count / totalTiles * 100.0f;
			entries.Add(new GenerationStatEntry { Name = Enum.GetName(enumType, value) ?? value.ToString(), Count = count, Percent = percent });
		}
	}

	private static void AppendIntGridSummaryLog(StringBuilder output, List<GenerationStatEntry> entries, string header)
	{
		output.AppendLine("== " + header + " ==");
		foreach(GenerationStatEntry entry in entries)
			output.AppendLine(entry.Name + ": " + entry.Count + " (" + entry.Percent.ToString("0.0") + "%)");
		output.AppendLine();
	}
}

using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public static class GenerationLogger
{
	public static void PrintGenerationLog(int width, int height, float[,] noiseMap, int[,] heightMap, int[,] resourceMap)
	{
		int chunkSize = GetGenerationLogChunkSize(width, height);
		StringBuilder output = new StringBuilder();

		output.AppendLine("==! GENERATION LOGGER !==");
		output.AppendLine();
		output.AppendLine("== DETAILS ==");
		output.AppendLine("Map size: " + width + "x" + height);
		output.AppendLine("Chunk size: " + chunkSize + "x" + chunkSize);
		output.AppendLine();

		AppendFloatChunkAverages(output, noiseMap, "Noice Map Chunk Averages", chunkSize);
		AppendIntGridSummary(output, heightMap, "Height Map Summary", typeof(DictionaryTileset.GenerateHeightType));
		AppendIntGridSummary(output, resourceMap, "Resource Map Summary", typeof(DictionaryTileset.GenerateResourceType));

		GD.Print(output.ToString());
	}

	private static int GetGenerationLogChunkSize(int width, int height)
	{
		const int targetChunkCount = 16;
		return Math.Max(1, (int)Math.Ceiling(Math.Max(width, height) / (float)targetChunkCount));
	}

	private static void AppendFloatChunkAverages(StringBuilder output, float[,] grid, string header, int chunkSize)
	{
		output.AppendLine("== " + header + " ==");
		List<float> chunkAverages = new List<float>();
		int chunkColumns = 0;

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
				chunkAverages.Add(average);
				currentChunkColumns++;

				output.Append(average.ToString("0.00"));
				output.Append(" ");
			}

			chunkColumns = Math.Max(chunkColumns, currentChunkColumns);
			output.AppendLine();
		}

		output.AppendLine();
		output.AppendLine("== " + header + " Visual ==");
		for(var i = 0; i < chunkAverages.Count; i++)
		{
			output.Append(GetFillChar(chunkAverages[i]));
			if((i + 1) % chunkColumns == 0)
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

	private static void AppendIntGridSummary(StringBuilder output, int[,] grid, string header, Type enumType)
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
}

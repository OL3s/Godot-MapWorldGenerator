using System.Collections.Generic;

public class GenerationStats
{
	public int Width { get; set; }
	public int Height { get; set; }
	public int ChunkSize { get; set; }
	public int NoiseColumns { get; set; }
	public List<float> NoiseChunkAverages { get; } = new List<float>();
	public List<GenerationStatEntry> HeightStats { get; } = new List<GenerationStatEntry>();
	public List<GenerationStatEntry> ResourceStats { get; } = new List<GenerationStatEntry>();
}

public class GenerationStatEntry
{
	public string Name { get; set; } = string.Empty;
	public int Count { get; set; }
	public float Percent { get; set; }
}

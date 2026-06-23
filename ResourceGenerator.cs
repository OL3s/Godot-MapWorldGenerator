using System;

public static class ResourceGenerator
{
	public static int[,] GenerateResourceMap(int[,] heightMap, float[,] foliageMap, float[,] mineralMap)
	{
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		int[,] resourceMap = new int[width, height];

		for(var x = 0; x < width; x++)
			for(var y = 0; y < height; y++)
				resourceMap[x, y] = GenerateResourceForHeight(heightMap[x, y], foliageMap[x, y], mineralMap[x, y]);

		return resourceMap;
	}

	private static int GenerateResourceForHeight(int heightType, float foliageValue, float mineralValue)
	{
		DictionaryTileset.GenerateHeightType height = (DictionaryTileset.GenerateHeightType)heightType;
		float foliageMultiplier = GetFoliageMultiplier(height);
		float mineralMultiplier = GetMineralMultiplier(height);
		if(foliageMultiplier == 0.0f && mineralMultiplier == 0.0f)
			return (int)DictionaryTileset.GenerateResourceType.None;

		float foliageHint = Math.Clamp(foliageValue, -1.0f, 1.0f);
		float foliageStrength = Math.Abs(foliageHint);
		float bushChance = 0.03f;
		float treeChance = 0.03f;

		if(foliageHint < 0.0f)
		{
			bushChance += foliageStrength * 0.45f;
			treeChance += foliageStrength * 0.12f;
		}
		else
		{
			treeChance += foliageStrength * 0.45f;
			bushChance += foliageStrength * 0.12f;
		}

		bushChance *= foliageMultiplier;
		treeChance *= foliageMultiplier;

		float mineralHint = Math.Clamp(mineralValue, -1.0f, 1.0f);
		float stoneChance = GetStoneChance(height, mineralHint) * mineralMultiplier;
		float mineralChance = GetMineralChance(height, mineralHint) * mineralMultiplier;

		bushChance = Math.Clamp(bushChance, 0.0f, 0.95f);
		treeChance = Math.Clamp(treeChance, 0.0f, 0.95f);
		stoneChance = Math.Clamp(stoneChance, 0.0f, 0.95f);
		mineralChance = Math.Clamp(mineralChance, 0.0f, 0.95f);

		double roll = Random.Shared.NextDouble();
		if(height == DictionaryTileset.GenerateHeightType.Mountain)
			return RollMountainResource(roll, stoneChance, mineralChance, bushChance, treeChance);

		return RollLandResource(roll, bushChance, treeChance, stoneChance, mineralChance);
	}

	private static int RollMountainResource(double roll, float stoneChance, float mineralChance, float bushChance, float treeChance)
	{
		if(roll < stoneChance)
			return (int)DictionaryTileset.GenerateResourceType.Stone;
		if(roll < stoneChance + mineralChance)
			return (int)DictionaryTileset.GenerateResourceType.Mineral;
		if(roll < stoneChance + mineralChance + bushChance)
			return (int)DictionaryTileset.GenerateResourceType.Bush;
		if(roll < stoneChance + mineralChance + bushChance + treeChance)
			return (int)DictionaryTileset.GenerateResourceType.Tree;

		return (int)DictionaryTileset.GenerateResourceType.None;
	}

	private static int RollLandResource(double roll, float bushChance, float treeChance, float stoneChance, float mineralChance)
	{
		if(roll < bushChance)
			return (int)DictionaryTileset.GenerateResourceType.Bush;
		if(roll < bushChance + treeChance)
			return (int)DictionaryTileset.GenerateResourceType.Tree;
		if(roll < bushChance + treeChance + stoneChance)
			return (int)DictionaryTileset.GenerateResourceType.Stone;
		if(roll < bushChance + treeChance + stoneChance + mineralChance)
			return (int)DictionaryTileset.GenerateResourceType.Mineral;

		return (int)DictionaryTileset.GenerateResourceType.None;
	}

	private static float GetStoneChance(DictionaryTileset.GenerateHeightType heightType, float mineralHint)
	{
		float stoneBias = Math.Clamp((mineralHint + 1.0f) / 2.0f, 0.0f, 1.0f);

		switch(heightType)
		{
			case DictionaryTileset.GenerateHeightType.Shore:
				return 0.02f + stoneBias * 0.10f;
			case DictionaryTileset.GenerateHeightType.Land:
				return 0.04f + stoneBias * 0.20f;
			case DictionaryTileset.GenerateHeightType.Mountain:
				return 0.15f + stoneBias * 0.75f;
			default:
				return 0.0f;
		}
	}

	private static float GetMineralChance(DictionaryTileset.GenerateHeightType heightType, float mineralHint)
	{
		float mineralBias = Math.Clamp(-mineralHint, 0.0f, 1.0f);

		switch(heightType)
		{
			case DictionaryTileset.GenerateHeightType.Shore:
				return 0.001f + mineralBias * 0.01f;
			case DictionaryTileset.GenerateHeightType.Land:
				return 0.003f + mineralBias * 0.02f;
			case DictionaryTileset.GenerateHeightType.Mountain:
				return 0.005f + mineralBias * 0.08f;
			default:
				return 0.0f;
		}
	}

	private static float GetFoliageMultiplier(DictionaryTileset.GenerateHeightType heightType)
	{
		switch(heightType)
		{
			case DictionaryTileset.GenerateHeightType.Shore:
				return 0.25f;
			case DictionaryTileset.GenerateHeightType.Land:
				return 1.0f;
			case DictionaryTileset.GenerateHeightType.Mountain:
				return 0.45f;
			default:
				return 0.0f;
		}
	}

	private static float GetMineralMultiplier(DictionaryTileset.GenerateHeightType heightType)
	{
		switch(heightType)
		{
			case DictionaryTileset.GenerateHeightType.Shore:
				return 0.25f;
			case DictionaryTileset.GenerateHeightType.Land:
				return 0.50f;
			case DictionaryTileset.GenerateHeightType.Mountain:
				return 1.25f;
			default:
				return 0.0f;
		}
	}
}

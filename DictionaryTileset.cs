using Godot;
using System.Collections.Generic;

public static class DictionaryTileset
{
    public static Dictionary<int, Vector2I> HeightMapAtlas = new Dictionary<int, Vector2I>() {
        { (int)GenerateHeightType.None,         new Vector2I(0, 0) }, 
        { (int)GenerateHeightType.DeepWater,    new Vector2I(1, 0) }, 
        { (int)GenerateHeightType.ShallowWater, new Vector2I(2, 0) }, 
        { (int)GenerateHeightType.Shore,        new Vector2I(3, 0) }, 
        { (int)GenerateHeightType.Lowland,      new Vector2I(0, 1) }, 
        { (int)GenerateHeightType.Highland,     new Vector2I(1, 1) }, 
        { (int)GenerateHeightType.Mountain,     new Vector2I(2, 1) }, 
        { (int)GenerateHeightType.SnowPeak,     new Vector2I(3, 1) }, 
    };
    public static Dictionary<int, Vector2I> ResourceMapAtlas = new Dictionary<int, Vector2I>() {
        { (int)GenerateResourceType.None,       new Vector2I(0, 0) }, 
        { (int)GenerateResourceType.Bush,       new Vector2I(1, 0) }, 
        { (int)GenerateResourceType.Tree,       new Vector2I(2, 0) }, 
        { (int)GenerateResourceType.Stone,      new Vector2I(3, 0) }, 
        { (int)GenerateResourceType.Mineral,    new Vector2I(1, 1) }, 
    };

    public enum GenerateHeightType {
        None = 0,
        DeepWater = 1,
        ShallowWater = 2,
        Shore = 3,
        Lowland = 4,
        Highland = 5,
        Mountain = 6,
        SnowPeak = 7,
    }

    public enum GenerateResourceType {
        None = 0,
        Bush = 1,
        Tree = 2,
        Stone = 3,
        Mineral = 4,
    }
}

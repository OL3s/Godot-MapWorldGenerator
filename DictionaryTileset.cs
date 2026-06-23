using Godot;
using System.Collections.Generic;

public static class DictionaryTileset
{
    public static Dictionary<int, Vector2I> HeightMapAtlas = new Dictionary<int, Vector2I>() {
        { (int)GenerateHeightType.None,         new Vector2I(0, 0) }, 
        { (int)GenerateHeightType.Ocean,        new Vector2I(1, 0) }, 
        { (int)GenerateHeightType.Water,        new Vector2I(2, 0) }, 
        { (int)GenerateHeightType.Shore,        new Vector2I(3, 0) }, 
        { (int)GenerateHeightType.Land,         new Vector2I(0, 1) }, 
        { (int)GenerateHeightType.Mountain,     new Vector2I(2, 1) }, 
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
        Ocean = 1,
        Water = 2,
        Shore = 3,
        Land = 4,
        Mountain = 5,
    }

    public enum GenerateResourceType {
        None = 0,
        Bush = 1,
        Tree = 2,
        Stone = 3,
        Mineral = 4,
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int level;
    public float size;
    public float spacing;

    public List<TileLayerData> tileLayer;
}
[Serializable]
public class TileLayerData
{
    public int layerSort;
    public float posY;
    public float posX;
    public int rows;
    public int cols;
    public string tileNameLayer;
    public List<Vector2Int> removeTile = new();


}

public enum FruitType
{
    Banana,
    Apple,
    Orange,
    Watermelon,
    Grapes,
    Cherry,
    Strawberry,
    Lemon,
    Pineapple,
    None
}

[Serializable]
public class TileSpriteData
{
    public FruitType fruitType;
    public Sprite sprite;
}

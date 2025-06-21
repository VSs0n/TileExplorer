using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    public int currentLevel;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private List<LevelData> tileLevel;
    [SerializeField] private List<TileSpriteData> spriteDataList;

    private readonly Dictionary<FruitType, int> m_Count = new Dictionary<FruitType, int>();
    private Dictionary<FruitType, Sprite> m_SpriteLookup;
    public int tileIndex{ get; set; }
    private float sizeToLoad { get; set; }
    private float spacingToLoad { get; set; }
    
    
    private List<FruitType> m_DistributedTiles;

    public readonly Dictionary<int, Tile[,]> m_LayerGrids = new();

    private readonly Dictionary<int, Transform> m_LayerParent = new();

    private readonly List<FruitType> m_ListFruit = new List<FruitType>()
    {  
        FruitType.Apple,
        FruitType.Banana, 
        FruitType.Grapes, 
        FruitType.Orange,
        FruitType.Watermelon,
        FruitType.Strawberry,
        FruitType.Pineapple,
        FruitType.Cherry,   
        FruitType.Lemon,
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    void Start()
    {
        GenerateTileManager();
    }

    private void GenerateTileManager()
    {
        m_SpriteLookup = new Dictionary<FruitType, Sprite>();
        foreach (var data in spriteDataList)
        {
            if (!m_SpriteLookup.ContainsKey(data.fruitType))
            {
                m_SpriteLookup.Add(data.fruitType, data.sprite);
            }
        }
        List<TileLayerData> layerConfigs = LoadLayerConfigs();

        int totalTileCount = 0;
        foreach (var config in layerConfigs)
        {
            int removeCount = config.removeTile != null ? config.removeTile.Count : 0;
            totalTileCount += (config.rows * config.cols) - removeCount;
            
        }
        CalculateIndexLayerTile(totalTileCount);

        LevelData levelData = tileLevel.Find(l => l.level == currentLevel);
        if (levelData != null)
        {
            sizeToLoad = levelData.size; 
            spacingToLoad = levelData.spacing; 
            foreach (var config in layerConfigs)
            {
                int maxLayer = 0;
                if (config.layerSort == layerConfigs.Count)
                {
                    maxLayer = config.layerSort;
                }
                Debug.Log("higher "+maxLayer);
                var tileConfig = GenerateTile(config.rows, config.cols, config.posY,config.posX, config.tileNameLayer, config.layerSort, config.removeTile, maxLayer);
                m_LayerGrids[config.layerSort] = tileConfig;
            }
        }
        

        Debug.Log("Total Tile: " + totalTileCount);
    }

    private List<TileLayerData> LoadLayerConfigs()
    {
        LevelData level = tileLevel.Find(l =>
        {
            if (l.level == currentLevel) return true;
            return false;
        });
        
        return level.tileLayer;
    }
    private void CalculateIndexLayerTile(int totalIndex)
    {
        m_DistributedTiles = GenerateDistributedTiles(totalIndex, m_ListFruit);
        ShuffleList(m_DistributedTiles);
         foreach (var fruitType in m_DistributedTiles)
        {
            if (!m_Count.ContainsKey(fruitType)) m_Count[fruitType] = 0;
            m_Count[fruitType]++;
        }

        foreach (var kv in m_Count)
        {
            Debug.Log($"{kv.Key}: {kv.Value}");
        }
    }
    private List<FruitType> GenerateDistributedTiles(int totalTilesCreated, List<FruitType> list)
    {
        List<int> itemCounts = GenerateBalancedCountsTile(totalTilesCreated, list.Count);
        ShuffleList(itemCounts);
        List<FruitType> result = new List<FruitType>();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < itemCounts[i]; j++)
            {
                result.Add(list[i]);
            }
        }

        return result;
    }

    private List<int> GenerateBalancedCountsTile(int totalTilesCreated, int listCount) 
     {
         List<int> countTiles = new List<int>();
         int baseCount = (totalTilesCreated / listCount) / 3 * 3;
         for (int i = 0; i < listCount; i++)
         {
             countTiles.Add(baseCount);
         }

         int used = baseCount * listCount;
         int remaining = totalTilesCreated - used;
         int extra = remaining / 3;

         List<int> indices = new List<int>();
         for (int i = 0; i < listCount; i++) indices.Add(i);
         for (int i = 0; i < extra; i++)
         {
             countTiles[indices[i]] += 3;
         }

         return countTiles;
     }


    private Tile[,] GenerateTile(int rows, int cols, float y,float x, string nameParentLayer, int orderInLayer,List<Vector2Int> removeIndex, int maxLayer)
    {
       
        Tile[,] tiles = new Tile[ cols, rows];
        float offset = (rows - 1) * spacingToLoad / 2f;
        float offsetY = (cols - 1) * spacingToLoad / 2f;
        
        if (!m_LayerParent.ContainsKey(orderInLayer))
        {
            GameObject layerParent = new GameObject(nameParentLayer);
            m_LayerParent[orderInLayer] = layerParent.transform;
            layerParent.transform.SetParent(this.transform);

        }
        
        for (int colY = 0; colY < tiles.GetLength(0); colY++)
        {
            for (int rowX = 0; rowX < tiles.GetLength(1); rowX++)
            {
                Vector3 spawnPosition = new Vector3((rowX * spacingToLoad - offset) +  x , -colY * spacingToLoad - offsetY + y, 0);
                if (removeIndex != null && removeIndex.Contains(new Vector2Int(rowX, colY)))
                {
                    continue;
                }

                var tile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                tile.transform.SetParent(m_LayerParent[orderInLayer]);
                
                tile.spriteFruit.sortingOrder = orderInLayer;
                tile.background.sortingOrder = orderInLayer - 1;

                tile.row = rowX;
                tile.col = colY;
                tile.currentLayer = orderInLayer;
                tile.tileManager = this;
                tile.isSelected = false;
       
                var fruit = GetDistributedFruitType();
                tile.fruitType = fruit;
                tile.spriteFruit.sprite = GetSpriteForFruitType(fruit);
                if (tile.spriteFruit.sprite == null)
                {
                    Debug.LogWarning($"Sprite null for fruitType: {fruit}");
                }
                tile.transform.localScale = new Vector2(sizeToLoad,sizeToLoad);

                tile.name = $"Tile_{colY}_{rowX}";
                tiles[colY, rowX] = tile;
            }
        }
        
        return tiles;
    }
    private FruitType GetDistributedFruitType()
    { 

        var fruit = m_DistributedTiles[tileIndex];
        tileIndex++;
        return fruit;
    }

    private Sprite GetSpriteForFruitType(FruitType type)
    {
        if (m_SpriteLookup.TryGetValue(type, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            return null;
        }
    }


    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
    }

    private FruitType GetFruitTypeFromSprite(Sprite sprite)
     {
         foreach (var kvp in m_SpriteLookup)
         {
             if (kvp.Value == sprite)
                 return kvp.Key;
         }

         return FruitType.None;
     }

    public bool IsTileCovered(Tile tile)
    {
        int[,] offsets = { { 0, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 } };

        foreach (var layer in m_LayerGrids)
        {
            int higherLayer = layer.Key;
            if (higherLayer <= tile.currentLayer) continue;

            Tile[,] higherGrid = layer.Value;
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                int checkCol = tile.col - offsets[i, 0];
                int checkRow = tile.row - offsets[i, 1];
            
                if (checkCol >= 0 && checkCol < higherGrid.GetLength(0) &&
                    checkRow >= 0 && checkRow < higherGrid.GetLength(1))
                {
                    Tile coveringTile = higherGrid[checkCol, checkRow];
                    if (coveringTile != null && !coveringTile.isSelected)
                    {
                      //  Debug.Log($"Tile ({tile.col},{tile.row}) Layer {tile.currentLayer} is covered by Tile ({checkCol},{checkRow}) Layer {higherLayer}");
                         return true;
                    }
                       
                }
            }
        }

        return false;
    }

    public void ShuffleTiles()
    {
        List<Tile> remainingTiles = new List<Tile>();
        List<FruitType> remainingFruitTypes = new List<FruitType>();
        
        foreach (var grid in m_LayerGrids.Values)
        {
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    Tile tile = grid[row, col];
                    if (tile != null && !tile.isSelected)
                    {
                        remainingTiles.Add(tile);
                        FruitType type = GetFruitTypeFromSprite(tile.spriteFruit.sprite);
                        remainingFruitTypes.Add(type);
                    }
                }
            }
        }

        ShuffleList(remainingFruitTypes);

        for (int i = 0; i < remainingTiles.Count; i++)
        {
            if (m_SpriteLookup.TryGetValue(remainingFruitTypes[i], out Sprite sprite))
            {
                remainingTiles[i].spriteFruit.sprite = sprite;
                remainingTiles[i].fruitType = remainingFruitTypes[i];

            }
        }
    }

    public bool HasNextLevel()
    {
        return tileLevel.Exists(level => level.level == currentLevel + 1);
    }

    public void NextLevel()
    {
        currentLevel += 1;
        m_DistributedTiles = null;
        m_Count.Clear();

        foreach (Transform layer in m_LayerParent.Values)
        {
           Destroy(layer.gameObject); 
        }
        m_LayerParent.Clear();
        m_LayerGrids.Clear();
        BoardTileCollector.Instance.ResetGame();
        GenerateTileManager();

    }

    public void ResetTile(int reviveLevel)
    {
        currentLevel = reviveLevel;
        tileIndex = 0;
        m_DistributedTiles = null;
        m_Count.Clear();

        foreach (Transform layer in m_LayerParent.Values)
        {
            Destroy(layer.gameObject); 
        }
        m_LayerParent.Clear();
        m_LayerGrids.Clear();
        BoardTileCollector.Instance.ResetGame();
        GenerateTileManager();

    }

    public void SetLayer(Tile tile, int layer)
    {
        tile.transform.SetParent(m_LayerParent[layer]);
    }
}
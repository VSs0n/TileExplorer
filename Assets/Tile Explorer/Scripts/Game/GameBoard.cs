using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBoard : MonoBehaviour
{
    [SerializeField] float cellSize = 1f;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileLayerData layerDataPrefab;
    [SerializeField] private BoardTileCollector boardTileCollector;

    public static event Action OnGameOver;
    public static event Action OnLevelCompleted;
        
    private List<TileLayerData> layers = new List<TileLayerData>();
        
    //private TileLevel currentLevelData;
    // public void LoadLevel(TileLevel levelData)
    // {
    //     currentLevelData = levelData;
    //     foreach (var layerData in levelData.layers)
    //     {
    //         var layer = Instantiate(layerPrefab, transform);
    //         layer.transform.localPosition = Vector3.zero;
    //         layers.Add(layer);
    //         foreach (var tileData in layerData.tiles)
    //         {
    //             var tileObject = Instantiate(tilePrefab, layer.transform);
    //             tileObject.SetData(tileData);
    //             layer.AddTileObject(tileObject);
    //             //TODO set tile position by x, y data
    //            tileObject.OnPicked = OnTilePicked;
    //         }
    //     }
    // }

    private void OnTilePicked(Tile tileObject)
    {
        boardTileCollector.AddTileObject(tileObject);
        //TODO Update tile locked status in lower layers
    }

    private void CheckGameOver(bool isGameOver)
    {
        if (isGameOver)
        {
            OnGameOver?.Invoke();
        }
        else
        {
            CheckLevelComplete();
        }
    }

    private void CheckLevelComplete()
    {
        //check all tiles are collected
        //OnLevelCompleted?.Invoke();
    }
    public void ShuffleBoard()
    {
        //TODO choose Fruit type by group 3 random tiles for same fruit
       // var fruitTypes = currentLevelData.fruitTypes;
    }
}

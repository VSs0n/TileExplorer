using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;


public class BoardTileCollector : MonoBehaviour
{
    public static BoardTileCollector Instance;
    public static event Action<int> completeLevel;
    [SerializeField] private List<Transform> slots;
    [SerializeField] private GameObject nextScreen;
    [SerializeField] private ReviveScreen reviveScreen;
    [SerializeField] private GameObject gameOver;

    private readonly List<Tile> collectedTiles = new List<Tile>();
    private List<Tile> originalTile = new List<Tile>();
    private int _tileCount = 0;
    private bool isProcessing = false ;
    private bool isSame = false;
    private bool isMoveSlot;
    private Vector2 originalScale;


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

    public void AddTileObject(Tile tile)
    {
        originalScale = tile.transform.lossyScale;
        tile.originalPosition = tile.transform.position;
        tile.originalLayer = tile.currentLayer;
        
        collectedTiles.Add(tile);
        originalTile.Add(tile);
        
        tile.spriteFruit.sortingOrder = 10;
        tile.background.sortingOrder = 9;
        
        var sameFruit = new List<Tile>();
        foreach (var tileObject in collectedTiles)
        {
            if (tileObject.fruitType == tile.fruitType)
            {
                sameFruit.Add(tileObject);
            }

        }
        var slotIndex = collectedTiles.Count;
        int takeCount = Mathf.Max(5, slotIndex);
        slotIndex = _tileCount;
        _tileCount++;

        tile.transform.parent = slots[slotIndex];
        if (sameFruit.Count == 2)
        {
            Debug.LogWarning("matching tiles");
            Tile firstTile = sameFruit[0];
            int targetIndex = collectedTiles.IndexOf(firstTile);

            if (targetIndex != -1)
            {
                Debug.LogWarning("Tile indices of matching tiles " + (targetIndex + 1));
                int targetTile = targetIndex + 1;
                slotIndex = targetTile;
                collectedTiles.Remove(tile);
                collectedTiles.Insert(targetIndex + 1, tile);
                tile.transform.DOMove(slots[slotIndex].position, 0.6f);
                ReArrangeTileObjects();
            }
        }
        if (sameFruit.Count == 3)
        {
            isProcessing = true;
            Debug.LogWarning("matching tiles");
            Tile firstTile = sameFruit[1];
            int targetIndex = collectedTiles.IndexOf(firstTile);

            if (targetIndex != -1)
            {
                Debug.LogWarning("Tile indices of matching tiles " + (targetIndex + 1));
                int targetTile = targetIndex + 1;
                slotIndex = targetTile;
                collectedTiles.Remove(tile);
                collectedTiles.Insert(targetIndex + 1, tile);
                tile.transform.DOMove(slots[slotIndex].position, 0.6f).OnComplete((() =>
                {
                    StartCoroutine(ScaleMatchingTiles(sameFruit));
                   
                }));
                ReArrangeTileObjects();
                
            }
        }
        tile.transform.DOMove(slots[slotIndex].position, 0.6f).SetLink(tile.gameObject).OnComplete(((() =>
        {
            if (!isProcessing)
            {
                if (collectedTiles.Count == slots.Count)
                {
                    Debug.Log("Revive");
                    reviveScreen.ShowCountDownTime(5f);
                }
            }
        })));
        tile.transform.DOScale(tile.transform.localScale + new Vector3(0.2f, 0.2f), 0.4f).SetLink(tile.gameObject)
            .OnComplete((() => { tile.transform.DOScale(1, 0.2f); }));
    }

    IEnumerator ScaleMatchingTiles(List<Tile> sameFruitTiles)
    {
        foreach (var tile in sameFruitTiles)
        {
            yield return tile.transform.DOScale(tile.transform.localScale + new Vector3(0.2f, 0.2f), 0.05f).SetEase(Ease.OutBack).WaitForCompletion();
            yield return new WaitForSeconds(0.01f);
        }
        
        foreach (var tile in sameFruitTiles)
        {
            collectedTiles.Remove(tile);
            _tileCount--;
            Destroy(tile.gameObject);
        }

        isProcessing = false;
        ReArrangeTileObjects();
        StartCoroutine(WaitAndCheckLevelEnd());
        
    }

    IEnumerator WaitAndCheckLevelEnd()
    {
        yield return new WaitForSeconds(0.3f);
        if (CheckTilesSelected())
        {
            if (TileManager.Instance.HasNextLevel())
            {
                Debug.Log("win");
                nextScreen.SetActive(true);
                TileManager.Instance.tileIndex = 0;
                completeLevel?.Invoke(TileManager.Instance.currentLevel);
            }
            else
            {            
                gameOver.SetActive(true);
                nextScreen.SetActive(false);
            }
            
        }
    }

    private void CollectTileObjects( )
    {  
        // posTileCollected.Add(position);
        // foreach (var pos in posTileCollected)
        // {
        //     Debug.LogWarning(pos);
        // }

        if (collectedTiles.Count == slots.Count)
        {
            Debug.Log("Revive");
            reviveScreen.ShowCountDownTime(5f);
        }
        if (CheckTilesSelected())
        {
            if (TileManager.Instance.HasNextLevel())
            {
                Debug.Log("win");
                nextScreen.SetActive(true);
                TileManager.Instance.tileIndex = 0;
                completeLevel?.Invoke(TileManager.Instance.currentLevel);
            }
            else
            {            
                gameOver.SetActive(true);
                nextScreen.SetActive(false);
            }
            
        }
    }
    
    private bool CheckTilesSelected()
    {
        var layerGrids = TileManager.Instance.m_LayerGrids;
        foreach (var grid in layerGrids.Values)
        {
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    Tile tile = grid[row, col];
                    if (tile != null && !tile.isSelected)
                    {
                        return false; 
                    }
                }
            }
        }
        return true;
        
    }
    private void ReArrangeTileObjects()
    {
        for (int i = 0; i < collectedTiles.Count; i++)
        {
            var tile = collectedTiles[i];
            tile.transform.parent = slots[i];
            tile.transform.DOMove(slots[i].position, 0.4f).SetLink(tile.gameObject);
        }
    }
    private void ReArrangeTileOriginal()
    {
        for (int i = 0; i < originalTile.Count; i++)
        {
            var tile = originalTile[i];
            tile.transform.parent = slots[i];
            tile.transform.DOMove(slots[i].position, 0.4f).SetLink(tile.gameObject);
        }
    }

    public void ResetGame()
    {
        foreach (var tileObject in collectedTiles)
        {
            Destroy(tileObject.gameObject);
        }

        _tileCount = 0;
        collectedTiles.Clear();
        originalTile.Clear();
    }

    public void RevertTiles(int numTiles)
    {
        int count = Mathf.Min(numTiles, originalTile.Count);

        for (int i = 0; i < count; i++)
        {
            int index = originalTile.Count - 1 - i;
            Tile tile = originalTile[index];


            tile.transform.DOMove(tile.originalPosition, 0.4f).SetEase(Ease.InOutQuad).OnComplete((() =>
            {
            }));
            tile.transform.DOScale(originalScale, 0.6f).OnComplete((() =>
            {
                if (numTiles > 1)
                {
                    TileManager.Instance.ShuffleTiles();
                    Debug.Log("Shuffle");
                }
                Debug.Log("Undo");
            }));
            tile.currentLayer = tile.originalLayer;
            tile.background.sortingOrder = tile.originalLayer - 1;
            tile.isSelected = false;
            tile.transform.SetParent(null);
            tile.collider2d.enabled = true;
            TileManager.Instance.SetLayer(tile, tile.originalLayer);
            collectedTiles.Remove(tile);

        }

        //  collectedTiles.Equals(originalTile);
        originalTile.RemoveRange(originalTile.Count - count, count);
       // collectedTiles.RemoveRange(collectedTiles.Count -numTiles, numTiles);
        _tileCount -= count;
       // ReArrangeTileOriginal();
       ReArrangeTileObjects();
    }


}
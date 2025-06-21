using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public SpriteRenderer spriteFruit;
    public SpriteRenderer background;
    public FruitType fruitType;

    public int col;
    public int row;
    public int currentLayer;
    public TileManager tileManager;
    public bool isSelected;
    public Collider2D collider2d;
    
    public Vector2 originalPosition;
    public int originalLayer;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TileManager.Instance.IsTileCovered(this))
        {
            return;
        }
        isSelected = true;
        collider2d.enabled = false;
        //Debug.Log($"Clicked tile Layer {currentLayer}");
        BoardTileCollector.Instance.AddTileObject(this);
    }
    
}

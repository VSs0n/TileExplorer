using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class TileObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;

    public FruitType fruitType;
    public bool isLocked;
    //public TileData Data;
    public Action<TileObject> OnPicked;
    public TileLayerData layerData;
    // public void SetFruit(FruitType type)
    // {
    //     fruitType = type;
    //     var spriteData =  TileManager.Instance.GetTileSpriteData(type);
    //     tileSprite.sprite = spriteData?.sprite;
    // }

    // public void SetData(TileData tileData)
    // {
    //     Data = tileData;
    // }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (!isLocked)
    //     {
    //         layer.RemoveTileObject(this);
    //         OnPicked?.Invoke(this);
    //     }
    // }

    public void UpdateLockedStatus(bool isLocked)
    {
        this.isLocked = isLocked;
    }
}

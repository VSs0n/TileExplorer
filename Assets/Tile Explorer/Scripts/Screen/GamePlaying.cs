using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GamePlaying : MonoBehaviour
{
   public static event Action<int> CurrentLevel; 
   [SerializeField] private Button refreshTile;
   [SerializeField] private Button back;
   [SerializeField] private Button reset;
   [SerializeField] private GameObject homeScreen;
   [SerializeField] private GameObject tileManager;
   [SerializeField] private GameObject tileCollector;
   [SerializeField] private TMP_Text currentLevel;
   [SerializeField] private RectTransform statusBar;
   private void Start()
   {
      refreshTile.onClick.AddListener((() => OnRefresh()));
      back.onClick.AddListener((() => OnBack()));
      reset.onClick.AddListener(Reset);

   }

   private void Reset()
   {
      BoardTileCollector.Instance.RevertTiles(1);
   }

   private void OnBack()
   {
      homeScreen.SetActive(true);
      this.gameObject.SetActive(false);
      tileManager.gameObject.SetActive(false);
      tileCollector.gameObject.SetActive(false);
      CurrentLevel?.Invoke(TileManager.Instance.currentLevel);
      statusBar.DOAnchorPos(new Vector2(0, 85),0.6f).SetEase(Ease.OutCubic);
   }

   private void OnRefresh()
   {
    //  Debug.Log("reset");
      TileManager.Instance.ShuffleTiles();
   }

   public void ShowStatusBar(int level)
   {
      gameObject.SetActive(true);
      statusBar.DOAnchorPos(new Vector2(0, -85),0.6f).SetEase(Ease.OutCubic);
      currentLevel.text = "Level " + level;
   }
}

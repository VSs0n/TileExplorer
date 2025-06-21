using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Button = UnityEngine.UI.Button;

public class NextScreen : MonoBehaviour
{
   
   [SerializeField] private Button buttonNextLevel;
   [SerializeField] private TMP_Text textCompleteLevel;
   [SerializeField] private TMP_Text textNextLevel;
   [SerializeField] private GamePlaying gamePlay;
   private int nextLevel;

   private void Start()
   {
      buttonNextLevel.onClick.AddListener((() => OnNextLevel()));
   }
   private void OnEnable()
   {
      BoardTileCollector.completeLevel += CompleteLevel;
   }
   private void OnDisable()
   {
      BoardTileCollector.completeLevel -= CompleteLevel;
   }
   
   private void CompleteLevel(int level)
   {
      nextLevel = level;
      textCompleteLevel.text = "Level " + nextLevel;
      textNextLevel.text = "Level " + (nextLevel + 1);
   }

   private void OnNextLevel()
   {
      gamePlay.ShowStatusBar(nextLevel + 1);
      this.gameObject.SetActive(false);
      TileManager.Instance.NextLevel();
   }
}

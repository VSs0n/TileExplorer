using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
  [SerializeField] private Button startOver;
  [SerializeField] private TMPro.TMP_Text textStartOver;

  private void Start()
  {
    startOver.onClick.AddListener((() => OnStartOver()));
    textStartOver.text = "Start Over Level " + 1;
  }

  private void OnStartOver()
  {
    this.gameObject.SetActive(false);
    TileManager.Instance.ResetTile(1);
  }
}

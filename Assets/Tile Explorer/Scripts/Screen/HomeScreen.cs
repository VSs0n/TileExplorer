using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    [SerializeField] private GamePlaying gamePlay;
    [SerializeField] private Button starGame;
    [SerializeField] private TMPro.TMP_Text textCurrentLevel;
    [SerializeField] private GameObject tileManager;
    [SerializeField] private GameObject tileCollector;
    private int currentLevel = 1;
    private void Start()
    {
        starGame.onClick.AddListener((() => StartGame()));
//        textCurrentLevel.text = "Level " + TileManager.Instance.currentLevel;

    }

    private void StartGame()
    {
        gamePlay.ShowStatusBar(currentLevel);
        tileManager.gameObject.SetActive(true);
        tileCollector.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GamePlaying.CurrentLevel += CurrentLevel;
    }
    private void OnDisable()
    {
        GamePlaying.CurrentLevel -= CurrentLevel;
    }
   
    private void CurrentLevel(int level)
    {
        currentLevel = level;
        textCurrentLevel.text = "Level " + currentLevel;
    }

}

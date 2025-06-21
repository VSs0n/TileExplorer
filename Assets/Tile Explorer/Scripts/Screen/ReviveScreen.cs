using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveScreen : MonoBehaviour
{
    [SerializeField] private Button revive;
    [SerializeField] private CountdownTime countdownTime;
    private void Start()
    {
        revive.onClick.AddListener((() => OnRevive()));
    }
    
    public void ShowCountDownTime(float duration)
    {
        gameObject.SetActive(true);
         StartCoroutine(countdownTime.UpdateTimer(duration, gameObject));
    }
    private void OnRevive()
    {
        this.gameObject.SetActive(false);
        BoardTileCollector.Instance.RevertTiles(5);

    }
}

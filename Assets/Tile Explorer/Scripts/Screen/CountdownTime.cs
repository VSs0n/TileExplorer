using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTime : MonoBehaviour
{
    [SerializeField] private Image fillTimer;
    [SerializeField] private TMP_Text timer;

    public IEnumerator UpdateTimer(float duration, GameObject screen)
    {
         float remainingDuration = duration;
        while (remainingDuration > 0)
        {
            remainingDuration -= Time.deltaTime;

            float clampedTime = Mathf.Max(0, remainingDuration);

            timer.text = clampedTime.ToString("0");

            fillTimer.fillAmount = clampedTime / duration;

            yield return null;
        }
        screen.SetActive(false);
        TileManager.Instance.ResetTile(TileManager.Instance.currentLevel);

    }
}
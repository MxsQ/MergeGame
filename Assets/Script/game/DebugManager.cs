using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public bool DevMode;

    [SerializeField] public GameObject GameDataUI;
    [SerializeField] public Text WarriorCountText;
    [SerializeField] public Text ArcherCountText;
    [SerializeField] public Text TotalHeroCount;
    [SerializeField] public Text WarTime;

    float _gameStartTime;

    void Awake()
    {
        Debug.Log("work?");
        if (DevMode)
        {
            GameDataUI.SetActive(true);
            GameManagers.OnGameEnd += OnGameEnd;
            GameManagers.OnGameStart += OnGameStart;
            GameManagers.OnGameFaild += OnGameFaild;
            GameManagers.OnGameWin += OnGameWin;
        }
        else
        {
            GameDataUI.SetActive(false);
        }
    }

    private void OnGameWin()
    {
        StopCoroutine("ShowWorkTime");
        WarTime.text = "War Time: " + (Time.time - _gameStartTime).ToString("#0.00") + "s";
    }

    private void OnGameFaild()
    {
        StopCoroutine("ShowWorkTime");
        WarTime.text = "War Time: " + (Time.time - _gameStartTime).ToString("#0.00") + "s";
    }

    private void OnGameStart()
    {
        _gameStartTime = Time.time;
        var record = GameManagers.Instance.PlayerRecored;
        WarriorCountText.text = "Warriror Count:  " + record.WarriorCount;
        ArcherCountText.text = "Archer Count:  " + record.ArcherCount;
        TotalHeroCount.text = "Total Hero:  " + (record.WarriorCount + record.ArcherCount);
        StartCoroutine("ShowWorkTime");
    }

    private void OnGameEnd()
    {
        float stopTime = Time.time - _gameStartTime;
    }

    IEnumerator ShowWorkTime()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            WarTime.text = "War Time: " + (Time.time - _gameStartTime).ToString("#0.00") + "s";
            yield return new WaitForSeconds(0.5f);
        }
    }
}

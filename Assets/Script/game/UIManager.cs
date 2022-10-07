using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject HeroPanel;
    [SerializeField] Text LevelText;
    [SerializeField] Text PlayerCoinsText;
    [SerializeField] Text AddWarriorText;
    [SerializeField] Text AddArcherText;
    [SerializeField] GameObject FinishPanel;
    [SerializeField] Text FinishPrompt;
    [SerializeField] Text GainConisText;

    private bool _inGame;
    private int _curLevel;
    private float _waitAddCoins;

    public static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;

        GameManagers.OnGameStart += OnGameStart;
        GameManagers.OnGameEnd += OnGameEnd;
        GameManagers.OnLevelChange += level => { LevelText.text = "Level " + level.ToString(); };
        GameManagers.OnGameWin += OnGameWin;
        GameManagers.OnGameFaild += OnGameFaild;

        LevelText.text = "Level " + GameManagers.Instance.PlayerRecored.Level.ToString();
        ChangePlayerCoinsShow();
        ChangeWarriorPriceShow();
        ChangeArcherPriceShow();


        StartCoroutine(DelayWorkOnAwake());
    }

    private IEnumerator DelayWorkOnAwake()
    {
        yield return new WaitForSeconds(0.1f);

        //LevelText.text = "Level " + GameManagers.Instance.PlayerRecored.Level.ToString();
    }

    public void OnAddWarriorClick()
    {
        MergeManager.Instance.OnAddWarriorClick();
    }

    public void OnAddArcherClick()
    {
        MergeManager.Instance.OnAddArcherClick();
    }

    public void OnGameStartClick()
    {
        GameManagers.Instance.InvokeGameStart();
    }

    public void OnGameEndClick()
    {
        GameManagers.Instance.InvokeGameEnd();
    }

    public void OnNextClick()
    {
        FinishPanel.SetActive(false);
        GameManagers.Instance.PlayerRecored.Coins += _waitAddCoins;
        _waitAddCoins = 0;
        ChangePlayerCoinsShow();
        GameManagers.Instance.InvokeGameEnd();
    }

    private void OnGameStart()
    {
        _curLevel = GameManagers.Instance.PlayerRecored.Level;
        _inGame = true; HeroPanel.SetActive(false);
    }

    private void OnGameEnd()
    {
        _inGame = false; HeroPanel.SetActive(true);
        _waitAddCoins = 0;
    }

    private void OnGameWin()
    {
        StartCoroutine(DeleyAction(1f, () =>
        {
            FinishPanel.SetActive(true);
        }));

        FinishPrompt.text = "Win";
        float percent = 1.0f - EvilManager.Instance.GetCurrentHPPercent();
        _waitAddCoins = Mathf.Round(LevelManager.Instance.GetLevelCoins(_curLevel) * percent);
        Debug.Log(percent + "   " + _waitAddCoins);
        ChangeGainCoinsShow(_waitAddCoins);
    }

    private void OnGameFaild()
    {
        StartCoroutine(DeleyAction(1f, () =>
        {
            FinishPanel.SetActive(true);
        }));

        FinishPrompt.text = "Faild";
        float percent = 1.0f - EvilManager.Instance.GetCurrentHPPercent();
        _waitAddCoins = Mathf.Round(LevelManager.Instance.GetLevelCoins(_curLevel) * percent);
        Debug.Log(percent + "   " + _waitAddCoins);
        ChangeGainCoinsShow(_waitAddCoins);
    }

    private IEnumerator DeleyAction(float deley, Action work)
    {
        yield return new WaitForSeconds(deley);

        work.Invoke();
    }

    public void ChangePlayerCoinsShow()
    {
        PlayerCoinsText.text = "Coins: " + GameManagers.Instance.PlayerRecored.Coins.ToString();
    }

    public void ChangeWarriorPriceShow()
    {
        AddWarriorText.text = "+战士：" + LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.WarriorCount).ToString();
    }

    public void ChangeArcherPriceShow()
    {
        AddArcherText.text = "+弓箭手：" + LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.ArcherCount).ToString();
    }

    private void ChangeGainCoinsShow(double coins)
    {
        GainConisText.text = "Gain: " + coins.ToString();
    }
}

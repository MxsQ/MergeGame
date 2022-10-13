using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject HeroPanel;
    [SerializeField] GameObject TopPanel;
    [SerializeField] Text LevelText;
    [SerializeField] Text PlayerCoinsText;
    [SerializeField] GameObject AddWarriorByCoinView;
    [SerializeField] GameObject AddWarriorByRewardView;
    [SerializeField] GameObject AddArcherByCoinView;
    [SerializeField] GameObject AddArcherByRewardView;
    [SerializeField] Text AddWarriorText;
    [SerializeField] Text AddArcherText;
    [SerializeField] GameObject FinishPanel;
    [SerializeField] Text FinishPrompt;
    [SerializeField] Text GainConisText;

    [SerializeField] Image[] Dots;
    [SerializeField] Sprite InProgressDot;
    [SerializeField] Sprite PassProgressDot;
    [SerializeField] Sprite UnreachProgressDot;


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
        GameManagers.OnLevelChange += OnLevelChange;
        GameManagers.OnGameWin += OnGameWin;
        GameManagers.OnGameFaild += OnGameFaild;

        //LevelText.text = "Level " + GameManagers.Instance.PlayerRecored.Level.ToString();
        ChangePlayerCoinsShow();
        ChangeWarriorBtnShow();
        ChangeArcherBtnShow();
        OnLevelChange(GameManagers.Instance.PlayerRecored.Level);

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

    public void OnAddWarriorByRewardClick()
    {

    }

    public void OnAddArcherClick()
    {
        MergeManager.Instance.OnAddArcherClick();
    }

    public void OnAddArcherByRewardClick()
    {

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
        TopPanel.SetActive(true);
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
        ChangeWarriorBtnShow();
        ChangeArcherBtnShow();
        ChangePlayerCoinsShow();

    }

    private void OnGameWin()
    {
        StartCoroutine(DeleyAction(1f, () =>
        {
            FinishPanel.SetActive(true);
            TopPanel.SetActive(false);
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
            TopPanel.SetActive(false);
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
        PlayerCoinsText.text = GetCoinStringWithUnit(GameManagers.Instance.PlayerRecored.Coins);
    }

    public void ChangeWarriorBtnShow()
    {
        var price = LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.WarriorCount);
        var playerCoins = GameManagers.Instance.PlayerRecored.Coins;
        AddWarriorText.text = GetCoinStringWithUnit(price);
        if (playerCoins >= price)
        {
            AddWarriorByCoinView.SetActive(true);
            AddWarriorByRewardView.SetActive(false);
        }
        else
        {
            AddWarriorByCoinView.SetActive(false);
            AddWarriorByRewardView.SetActive(true);
        }
    }

    public void ChangeArcherBtnShow()
    {
        var price = LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.WarriorCount);
        var playerCoins = GameManagers.Instance.PlayerRecored.Coins;
        AddArcherText.text = GetCoinStringWithUnit(LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.ArcherCount));
        if (playerCoins >= price)
        {
            AddArcherByCoinView.SetActive(true);
            AddArcherByRewardView.SetActive(false);
        }
        else
        {
            AddArcherByCoinView.SetActive(false);
            AddArcherByRewardView.SetActive(true);
        }
    }

    private void ChangeGainCoinsShow(double coins)
    {
        GainConisText.text = "Gain: " + GetCoinStringWithUnit(coins);
    }


    private string[] _coinUnit = new string[] { "", "K", "M", "G", "T", "P", "E" };

    private string GetCoinStringWithUnit(double coin)
    {
        double showNum = coin;
        var unitIndex = 0;
        while (showNum > 1000)
        {
            showNum /= 1000;
            unitIndex++;
        }

        if (unitIndex == 0)
        {
            return showNum.ToString();
        }
        else if (showNum * 10 % 10 == 0)
        {
            return showNum.ToString("0") + " " + _coinUnit[unitIndex];
        }

        return showNum.ToString("0.0") + " " + _coinUnit[unitIndex];
    }

    private void OnLevelChange(int level)
    {
        LevelText.text = "Level " + level.ToString();

        var curLevelIndex = level % 10;

        //if (level < 10 && curLevelIndex == 0)
        //{
        //    foreach (Image img in Dots)
        //    {
        //        img.sprite = GameObject.Instantiate(UnreachProgressDot);
        //    }
        //}
        //else
        //{
        for (int i = 0; i < curLevelIndex - 1; i++)
        {
            Dots[i].sprite = GameObject.Instantiate(PassProgressDot);
        }

        Dots[curLevelIndex - 1].sprite = GameObject.Instantiate(InProgressDot);

        for (int i = curLevelIndex; i < 9; i++)
        {
            Dots[i].sprite = GameObject.Instantiate(UnreachProgressDot);
        }
        //}



    }
}

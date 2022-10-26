using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject WarPage;
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

    [SerializeField] ClearUIManager ClearUI;
    [SerializeField] SkinManager SkinUI;


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
        ClearUI.Hide();
        GameManagers.Instance.PlayerRecored.Coins += _waitAddCoins;
        _waitAddCoins = 0;
        ChangePlayerCoinsShow();
        GameManagers.Instance.InvokeGameEnd();
        TopPanel.SetActive(true);
    }

    public void OnShowSkinPageClick()
    {
        WarPage.SetActive(false);
        TopPanel.SetActive(false);
        HeroPanel.SetActive(false);
        SkinUI.Show();
    }

    public void OnCloseSkinPageClick()
    {
        WarPage.SetActive(true);
        TopPanel.SetActive(true);
        HeroPanel.SetActive(true);
        SkinUI.Hide();
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
            ShowClearUI();
        }));
    }

    private void OnGameFaild()
    {
        StartCoroutine(DeleyAction(1f, () =>
        {
            ShowClearUI();
        }));
    }

    private void ShowClearUI()
    {
        float percent = 1.0f - EvilManager.Instance.GetCurrentHPPercent();
        _waitAddCoins = Mathf.Round(LevelManager.Instance.GetLevelCoins(_curLevel) * percent);
        Debug.Log(percent + "   " + _waitAddCoins);

        TopPanel.SetActive(false);
        ClearUI.Show(true, _waitAddCoins);

    }

    private IEnumerator DeleyAction(float deley, Action work)
    {
        yield return new WaitForSeconds(deley);

        work.Invoke();
    }

    public void ChangePlayerCoinsShow()
    {
        PlayerCoinsText.text = TextUtils.GetCoinStringWithUnit(GameManagers.Instance.PlayerRecored.Coins);
    }

    public void ChangeWarriorBtnShow()
    {
        var price = LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.WarriorCount);
        var playerCoins = GameManagers.Instance.PlayerRecored.Coins;
        AddWarriorText.text = TextUtils.GetCoinStringWithUnit(price);
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
        AddArcherText.text = TextUtils.GetCoinStringWithUnit(LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.ArcherCount));
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

    private void OnLevelChange(int level)
    {
        Debug.Log("to level " + level.ToString());
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

        if (curLevelIndex == 0)
        {
            foreach (Image d in Dots)
            {
                d.sprite = GameObject.Instantiate(PassProgressDot);
            }
            return;
        }

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

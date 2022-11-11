using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject WarPage;
    [SerializeField] GameObject HeroPanel;
    [SerializeField] GameObject TopPanel;
    [SerializeField] GameObject SkinEntrance;
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
    [SerializeField] SpriteRenderer GameBG;


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
        AudioManager.Instance.PlayClick();
        MergeManager.Instance.OnAddWarriorClick();
    }

    public void OnAddWarriorByRewardClick()
    {
        AudioManager.Instance.PlayClick();
    }

    public void OnAddArcherClick()
    {
        AudioManager.Instance.PlayClick();
        MergeManager.Instance.OnAddArcherClick();
    }

    public void OnAddArcherByRewardClick()
    {
        AudioManager.Instance.PlayClick();
    }


    public void OnGameStartClick()
    {
        AudioManager.Instance.PlayClick();
        GameManagers.Instance.InvokeGameStart();
    }

    public void OnGameEndClick()
    {
        GameManagers.Instance.InvokeGameEnd();
    }

    public void OnNextClick()
    {
        _waitAddCoins = 0;
        ClearUI.Hide(() =>
        {
            ChangePlayerCoinsShow();
            GameManagers.Instance.InvokeGameEnd();
            TopPanel.SetActive(true);
        });
    }

    public void OnShowSkinPageClick()
    {
        AudioManager.Instance.PlayClick();
        WarPage.SetActive(false);
        TopPanel.SetActive(false);
        HeroPanel.SetActive(false);
        SkinEntrance.SetActive(false);
        SkinUI.Show();
    }

    public void OnCloseSkinPageClick()
    {
        AudioManager.Instance.PlayClick();
        WarPage.SetActive(true);
        TopPanel.SetActive(true);
        HeroPanel.SetActive(true);
        SkinEntrance.SetActive(true);
        SkinUI.Hide();
    }

    private void OnGameStart()
    {
        _curLevel = GameManagers.Instance.PlayerRecored.Level;
        _inGame = true; HeroPanel.SetActive(false);
        SkinEntrance.SetActive(false);
    }

    private void OnGameEnd()
    {
        _inGame = false; HeroPanel.SetActive(true);
        _waitAddCoins = 0;
        ChangeWarriorBtnShow();
        ChangeArcherBtnShow();
        ChangePlayerCoinsShow();
        SkinEntrance.SetActive(true);
    }

    private void OnGameWin()
    {
        StartCoroutine(DeleyAction(1f, () =>
        {
            ShowClearUI(true);
        }));
    }

    private void OnGameFaild()
    {
        StartCoroutine(DeleyAction(1f, () =>
        {
            ShowClearUI(false);
        }));
    }

    private void ShowClearUI(bool win)
    {
        float percent = 1.0f - EvilManager.Instance.GetCurrentHPPercent();
        _waitAddCoins = Mathf.Round(LevelManager.Instance.GetLevelCoins(_curLevel) * percent);
        Debug.Log(percent + "   " + _waitAddCoins);

        TopPanel.SetActive(false);
        ClearUI.Show(win, _waitAddCoins);

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
        var price = LevelManager.Instance.GetRolePriceBy(GameManagers.Instance.PlayerRecored.ArcherCount);
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

        // change bg
        var levelBG = GameManagers.Instance.LevelBG;
        var bgIndex = level / 10 % levelBG.Length;
        GameBG.sprite = GameObject.Instantiate(levelBG[bgIndex]);

    }
}

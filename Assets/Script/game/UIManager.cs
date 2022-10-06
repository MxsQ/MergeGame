using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject HeroPanel;
    [SerializeField] Text LevelText;
    [SerializeField] GameObject FinishPanel;
    [SerializeField] Text FinishPrompt;

    private bool _inGame;

    private void Awake()
    {
        GameManagers.OnGameStart += OnGameStart;
        GameManagers.OnGameEnd += OnGameEnd;
        GameManagers.OnLevelChange += level => { LevelText.text = "Level " + level.ToString(); };
        GameManagers.OnGameWin += OnGameWin;
        GameManagers.OnGameFaild += OnGameFaild;

        StartCoroutine(DelayWorkOnAwake());
    }

    private IEnumerator DelayWorkOnAwake()
    {
        yield return new WaitForSeconds(0.1f);

        LevelText.text = "Level " + GameManagers.Instance.PlayerRecored.Level.ToString();
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
        GameManagers.Instance.InvokeGameEnd();
        FinishPanel.SetActive(false);
    }

    private void OnGameStart()
    {
        _inGame = true; HeroPanel.SetActive(false);
    }

    private void OnGameEnd()
    {
        _inGame = false; HeroPanel.SetActive(true);
    }

    private void OnGameWin()
    {
        FinishPanel.SetActive(true);
        FinishPrompt.text = "Win";
    }

    private void OnGameFaild()
    {
        FinishPanel.SetActive(true);
        FinishPrompt.text = "Faild";
    }
}

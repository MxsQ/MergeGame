using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject HeroPanel;

    private bool _inGame;

    private void Awake()
    {
        GameManagers.OnGameStart += () => { _inGame = true; HeroPanel.SetActive(false); };
        GameManagers.OnGameEnd += () => { _inGame = false; HeroPanel.SetActive(true); };
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
        GameManagers.Instance.InvodeGameStart();
    }

    public void OnGameEndClick()
    {
        GameManagers.Instance.InvodeGameEnd();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvilManager : MonoBehaviour
{
    [SerializeField] GameObject EvilPositionMid1;
    [SerializeField] GameObject EvilPositionMid2;
    [SerializeField] GameObject EvilPositionBig1;

    [SerializeField] EvilItem EvilMid1;
    [SerializeField] EvilItem EvilMid2;
    [SerializeField] EvilItem EvilBig1;

    [SerializeField] Image BloodBar;

    [SerializeField] GameObject Evil;

    //private GameObject e1;
    //private GameObject e2;
    //private GameObject e3;

    private int _bloodBarSize = 400;
    public float BlooadPercent = 100;

    //private const int MID_SCALE = 100;
    private static EvilManager _instance;

    public static EvilManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
        GameManagers.OnGameStart += OnGameStart;
        GameManagers.OnGameEnd += OnGameEnd;
        GameManagers.OnGameWin += () =>
        {
            var sizeDate = BloodBar.rectTransform.sizeDelta;
            BloodBar.rectTransform.sizeDelta = new Vector2(0, sizeDate.y);
        };

        LoadEvil();
        //CreateEvil();
    }

    private void LoadEvil()
    {
        EvilMid1.Reset();
        EvilMid2.Reset();
        EvilBig1.Reset();

        List<EnemyUnit> es = LevelManager.Instance.GetLevelEvils();
        Debug.Log("Load eveil");
        if (es.Count == 2)
        {
            EvilMid1.set(es[0]);
            EvilMid2.set(es[1]);
        }
        else
        {
            EvilBig1.set(es[0]);
        }
    }

    private void Update()
    {
        if (GameManagers.InGame)
        {
            EvilMid1.DoUpdate();
            EvilMid2.DoUpdate();
            EvilBig1.DoUpdate();
        }
    }

    private void LateUpdate()
    {
        if (GameManagers.InGame)
        {
            EvilMid1.DoLateUpdate();
            EvilMid2.DoLateUpdate();
            EvilBig1.DoLateUpdate();
        }
    }

    private void OnGameEnd()
    {
        LoadEvil();
        var sizeDate = BloodBar.rectTransform.sizeDelta;
        BloodBar.rectTransform.sizeDelta = new Vector2(_bloodBarSize, sizeDate.y);
    }

    private void OnGameStart()
    {
        //EvilMid1.MakeReady();
        //EvilMid2.MakeReady();
        //EvilBig1.MakeReady();
    }

    public void OnEvilBeHit()
    {

        float percent = GetCurrentHPPercent();
        percent = percent < 0 ? 0 : percent;
        //Debug.Log("maxHP=" + maxHP + "  curHP=" + curHP);
        BlooadPercent = percent;
        var sizeDate = BloodBar.rectTransform.sizeDelta;
        BloodBar.rectTransform.sizeDelta = new Vector2(_bloodBarSize * percent, sizeDate.y);
    }

    public float GetCurrentHPPercent()
    {
        int maxHP = LevelManager.Instance.GetLevelHP();
        int curHP = EvilMid1.GetRoleHP();
        curHP += EvilMid2.GetRoleHP();
        curHP += EvilBig1.GetRoleHP();

        //if(EvilMid2)

        //Debug.Log("maxHP=" + maxHP + "  curHP=" + curHP
        //    + " m1=" + EvilMid1.GetRoleHP() + " m2=" + EvilMid2.GetRoleHP() + " big=" + EvilBig1.GetRoleHP()); ;

        float percent = curHP * 1.0f / maxHP;

        return percent;
    }
}

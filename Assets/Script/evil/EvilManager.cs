using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilManager : MonoBehaviour
{
    [SerializeField] GameObject EvilPositionMid1;
    [SerializeField] GameObject EvilPositionMid2;
    [SerializeField] GameObject EvilPositionBig1;

    [SerializeField] EvilItem EvilMid1;
    [SerializeField] EvilItem EvilMid2;
    [SerializeField] EvilItem EvilBig1;

    [SerializeField] GameObject BloodBar;

    [SerializeField] GameObject Evil;

    //private GameObject e1;
    //private GameObject e2;
    //private GameObject e3;

    private int _bloodBarSize = 1000;

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
        GameManagers.OnGameEnd += LoadEvil;
        GameManagers.OnGameWin += () =>
        {
            var scale = BloodBar.transform.localScale;
            BloodBar.transform.localScale = new Vector3(0, scale.y, scale.z);
        };

        LoadEvil();
        //CreateEvil();
    }

    private void LoadEvil()
    {
        EvilMid1.Reset();
        EvilMid2.Reset();
        EvilBig1.Reset();

        List<GameObject> es = LevelManager.Instance.GetLevelEvils();
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

    //private void CreateEvil()
    //{
    //    var minScale = GameManagers.Instance.Config.EvilMinBoxRadius;
    //    GameObject evil = GameObject.Instantiate(Evil);
    //    evil.transform.parent = EvilMid1.gameObject.transform;
    //    evil.transform.localScale = new Vector3(minScale, minScale, 1);
    //    evil.transform.localEulerAngles = new Vector3(0, -180, 0);
    //    evil.transform.localPosition = new Vector3(0, -100, 0);

    //    e1 = evil;
    //}

    private void OnGameStart()
    {
        //EvilMid1.set(e1);
        var scale = BloodBar.transform.localScale;
        BloodBar.transform.localScale = new Vector3(_bloodBarSize, scale.y, scale.z);
    }

    private void MakeEvilReady()
    {
        //EvilMid1.RebuildEviel();
        //CreateEvil();
    }

    public void OnEvilBeHit()
    {

        float percent = GetCurrentHPPercent();
        var scale = BloodBar.transform.localScale;
        //Debug.Log("maxHP=" + maxHP + "  curHP=" + curHP);
        BloodBar.transform.localScale = new Vector3(_bloodBarSize * percent, scale.y, scale.z);
    }

    public float GetCurrentHPPercent()
    {
        int maxHP = LevelManager.Instance.GetLevelHP();
        int curHP = EvilMid1.GetRoleHP();
        curHP += EvilMid2.GetRoleHP();
        curHP += EvilBig1.GetRoleHP();
        float percent = curHP * 1.0f / maxHP;

        return percent;
    }
}

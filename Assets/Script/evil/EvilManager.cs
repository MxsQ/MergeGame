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

    private GameObject e1;
    private GameObject e2;
    private GameObject e3;

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
        GameManagers.OnGameStart += MakeEvilWork;
        GameManagers.OnGameEnd += MakeEvilReady;
        CreateEvil();
    }

    private void CreateEvil()
    {
        var minScale = GameManagers.Instance.Config.EvilMinBoxRadius;
        GameObject evil = GameObject.Instantiate(Evil);
        evil.transform.parent = EvilMid1.gameObject.transform;
        evil.transform.localScale = new Vector3(minScale, minScale, 1);
        evil.transform.localEulerAngles = new Vector3(0, -180, 0);
        evil.transform.localPosition = new Vector3(0, -100, 0);

        e1 = evil;
    }

    private void MakeEvilWork()
    {
        EvilMid1.set(e1);
        var scale = BloodBar.transform.localScale;
        BloodBar.transform.localScale = new Vector3(_bloodBarSize, scale.y, scale.z);
    }

    private void MakeEvilReady()
    {
        EvilMid1.RebuildEviel();
        CreateEvil();
    }

    public void OnEvilBeHit()
    {
        int maxHP = LevelManager.Instance.GetLevelHP();
        int curHP = EvilMid1.GetRoleHP();
        float percent = curHP * 1.0f / maxHP;
        var scale = BloodBar.transform.localScale;
        Debug.Log("maxHP=" + maxHP + "  curHP=" + curHP);
        BloodBar.transform.localScale = new Vector3(_bloodBarSize * percent, scale.y, scale.z);
    }
}

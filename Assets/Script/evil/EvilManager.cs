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

    [SerializeField] GameObject Evil;

    private GameObject e1;
    private GameObject e2;
    private GameObject e3;

    private const int MID_SCALE = 100;

    private void Awake()
    {
        GameManagers.OnGameStart += MakeEvilWork;
        CreateEvil();
    }

    private void CreateEvil()
    {
        GameObject evil = GameObject.Instantiate(Evil);
        evil.transform.parent = EvilMid1.gameObject.transform;
        evil.transform.localScale = new Vector3(MID_SCALE, MID_SCALE, 1);
        evil.transform.localEulerAngles = new Vector3(0, -180, 0);
        evil.transform.localPosition = new Vector3(0, -100, 0);

        e1 = evil;
    }

    private void MakeEvilWork()
    {
        EvilMid1.set(e1);
    }
}

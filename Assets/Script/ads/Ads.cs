using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ads : MonoBehaviour, IADManager
{
    private static Ads _instance;

    public static Ads Instance
    {
        get { return _instance; }
    }

    private IADManager _ad;

    private void Awake()
    {
        _instance = this;
        _ad = new FakeAdManager();
        _ad.Initialize();
    }

    public void Initialize()
    {

    }

    public void ShowRV(Action<bool> OnComplete)
    {
        _ad.ShowRV(OnComplete);
    }
}

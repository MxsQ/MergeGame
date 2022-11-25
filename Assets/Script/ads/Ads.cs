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
        //#if UNITY_ANDROID
        _ad = new AndroidAdManager();
        //#else
        //_ad = new FakeAdManager();
        //#endif
        _ad.Initialize();
        DontDestroyOnLoad(this);
    }

    public void Initialize()
    {

    }

    public void ShowRV(Action<bool> OnComplete)
    {
        _ad.ShowRV(OnComplete);
    }

    public void onEvent(string msg)
    {
        _ad.onEvent(msg);
    }
}

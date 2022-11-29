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
    private IAnalytics _analytics;

    private void Awake()
    {
        _instance = this;
#if UNITY_EDITOR
        _ad = new FakeAdManager();
        _analytics = new FakeAnalytics();
#else
        _ad = new AndroidAdManager();
        _analytics = new HWAnalytics();
#endif
        _ad.Initialize();
        _analytics.Initialize();
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

    public void report(string eventName)
    {
        _analytics.OnEvent(eventName);
    }

    public void ShowNativeRv(Action<bool> OnComplete)
    {
        _ad.ShowNativeRv(OnComplete);
    }
}

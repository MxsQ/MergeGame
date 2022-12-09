using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AndroidAdManager;

public class NativeAd : IAndroidAd
{
    private const string CHECK_CHACHE_FUNCTION = "unityCheckExpressRVCache";
    private const string SHOW_NATIVE_FUCTION = "unityCallShowExpressRV";

    AndroidJavaClass _javaclass;

    Action<bool> OnComplete;

    bool _reward = false;

    public void Dealwith(AdMessage ad)
    {
        Debug.Log("NativeAd deal with: " + ad.adEvent);
        if (ad.adEvent == CLOSE_EVENT)
        {
            // note: make sure only one block response to here
            _reward = true;
            OnComplete?.Invoke(_reward);
            OnComplete = null;
        }
    }

    public void SetJavaClass(AndroidJavaClass javaClass)
    {
        _javaclass = javaClass;
    }

    public void Show(Action<bool> onComplete)
    {
        if (_javaclass.CallStatic<bool>(CHECK_CHACHE_FUNCTION))
        {
            _reward = false;
            OnComplete = onComplete;
            _javaclass.CallStatic(SHOW_NATIVE_FUCTION);
        }
    }

    public void ShowCenterNative()
    {
        _javaclass.CallStatic("showCenterExpress");
    }
}

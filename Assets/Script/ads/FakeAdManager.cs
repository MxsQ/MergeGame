using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeAdManager : IADManager
{
    public void Initialize()
    {
    }

    public void onEvent(string msg)
    {

    }

    public void ShowNativeRv(Action<bool> OnComplete)
    {
        OnComplete?.Invoke(true);
    }

    public void ShowRV(Action<bool> OnComplete)
    {
        OnComplete?.Invoke(true);
    }
}

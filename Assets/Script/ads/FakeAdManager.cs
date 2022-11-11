using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeAdManager : IADManager
{
    public void Initialize()
    {
    }

    public void ShowRV(Action<bool> OnComplete)
    {
        OnComplete?.Invoke(true);
    }
}

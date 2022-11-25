using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IADManager
{
    public void Initialize();

    public void onEvent(string msg);
    public void ShowRV(Action<bool> OnComplete);
}

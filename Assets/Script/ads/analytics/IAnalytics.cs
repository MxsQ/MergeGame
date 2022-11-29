using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAnalytics
{
    public abstract void Initialize();

    public abstract void OnEvent(string evenName);
}

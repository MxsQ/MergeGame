using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HWAnalytics : IAnalytics
{
    private AndroidJavaClass _bridge;
    private const string BRIDGE_CLASS_NAME = "com.mg.publish.hive.UnityReportBridge";
    private const string EVENT_FUCTION = "onEvent";
    private const string ANALYTICS_NAME = "Huawei";

    public override void Initialize()
    {
        _bridge = new AndroidJavaClass(BRIDGE_CLASS_NAME);
    }

    public override void OnEvent(string evenName)
    {
        _bridge?.CallStatic(EVENT_FUCTION, ANALYTICS_NAME, evenName);
    }
}

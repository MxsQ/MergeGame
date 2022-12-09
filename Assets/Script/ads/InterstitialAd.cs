using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AndroidAdManager;

public class InterstitialAd : IAndroidAd
{
    AndroidJavaClass _javaclass;

    public void Dealwith(AdMessage ad)
    {
        Debug.Log("Interstitial deal with: " + ad.adEvent);

        if (ad.adEvent == LOAD_FAILED_EVENT)
        {
            GameManagers.Instance.StartCoroutine(TryReload(15f));
        }
    }

    public void SetJavaClass(AndroidJavaClass javaClass)
    {
        _javaclass = javaClass;
    }

    IEnumerator TryReload(float deley)
    {
        yield return new WaitForSeconds(deley);

        load();
    }

    public void load()
    {
        _javaclass.CallStatic("unityCallLoadInterstitial");
    }

    public void show()
    {
        _javaclass.CallStatic("showInterstitial");
    }
}

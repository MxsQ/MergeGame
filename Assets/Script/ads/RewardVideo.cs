using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AndroidAdManager;

public class RewardVideo : IAndroidAd
{
    AndroidJavaClass _javaclass;

    Action<bool> OnComplete;

    bool _reward = false;

    public void Dealwith(AdMessage ad)
    {
        Debug.Log("Reward Video deal with: " + ad.adEvent);
        if (ad.adEvent == LOAD_FAILED_EVENT)
        {
            GameManagers.Instance.StartCoroutine(TryReload());
        }
        else if (ad.adEvent == SHOW_FAILED_EVENT)
        {
            OnComplete?.Invoke(_reward);
            OnComplete = null;
        }
        else if (ad.adEvent == CLOSE_EVENT)
        {
            OnComplete?.Invoke(_reward);
            OnComplete = null;
        }
        else if (ad.adEvent == REWARD_EVENT)
        {
            string r = ad.allParams[REWARD_STATUES];
            _reward = r == null ? false
                : r.Equals("true") ? true
                : r.Equals("false") ? false
                : false;
            _reward = true;
        }
    }

    public void SetJavaClass(AndroidJavaClass javaClass)
    {
        _javaclass = javaClass;
    }

    IEnumerator TryReload()
    {
        yield return new WaitForSeconds(15f);

        load();
    }

    public void load()
    {
        _javaclass.CallStatic("unityCallLoadRewardVideo");
    }

    public void show(Action<bool> onComplete)
    {
        _reward = false;
        OnComplete = onComplete;
        _javaclass.CallStatic("showRewardVideo");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidAdManager : IADManager
{

    public static string PACKAGE = "com.mg.publish.hive";
    public static string ADS_BRIDGE_CLASS = PACKAGE + ".UnityAdsBridge";
    public static string STASTIC_BRIDGE_CLASS = PACKAGE + ".UnityReportBridge";

    public const string CACHE_EVENT = "cache";
    public const string LOAD_FAILED_EVENT = "loadFailed";
    public const string SHOW_EVENT = "show";
    public const string SHOW_FAILED_EVENT = "showFailed";
    public const string CLICK_EVENT = "click";
    public const string CLOSE_EVENT = "close";
    public const string REWARD_EVENT = "reward";
    public const string REWARD_STATUES = "reward_statue";

    public const string VIDEO = "video";
    public const string INTERSTITIAL = "interstitial";
    public const string BANNER = "banner";
    public const string EXPRESS = "express";
    public const string SPLASH = "splash";

    public RewardVideo _rewardVideo;


    private AndroidJavaClass _AdsBridge;
    private Dictionary<string, AdAction> adActions = new Dictionary<string, AdAction>();

    public AndroidJavaClass bridgeClass() => _AdsBridge;

    public void Initialize()
    {
        try
        {
            Console.WriteLine("start to load java class");
            Debug.Log("start to load java class");
            _AdsBridge = new AndroidJavaClass(ADS_BRIDGE_CLASS);
            //_stastic = MobgiStastic.Instance;

            _rewardVideo = new RewardVideo();
            _rewardVideo.SetJavaClass(_AdsBridge);
            _rewardVideo.load();
        }
        catch (Exception e)
        {
            Console.WriteLine($"获取java类错啦 {e.Message}");
            Debug.Log($"获取java类错啦 {e.Message}");
        }
    }

    public void onEvent(string msg)
    {
        Debug.Log($"receive ad event : {msg}");
        AdMessage ad = AdMessage.parse(msg);
        if (ad.type == VIDEO)
        {
            _rewardVideo.Dealwith(ad);
        }
    }


    public void ShowRV(Action<bool> OnComplete)
    {
        _rewardVideo.show(OnComplete);
    }

    public interface IAndroidAd
    {
        public void SetJavaClass(AndroidJavaClass javaClass);

        public void Dealwith(AdMessage ad);
    }
}

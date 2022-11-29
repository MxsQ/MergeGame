using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearUIManager : MonoBehaviour
{
    [SerializeField] Animator ClearUIAnimator;
    [SerializeField] GameObject FinishPanel;
    [SerializeField] Text GainConisText;
    [SerializeField] Text PlayerConinsTextInFinishUI;
    [SerializeField] Text RewardCoinsText;
    [SerializeField] Image TotalDpsBar;
    [SerializeField] Text TotalDpsText;
    [SerializeField] GameObject Pointer;
    [SerializeField] Transform WheelCenter;

    [SerializeField] Image StatusBar;
    [SerializeField] Image StatusBG;
    [SerializeField] Sprite WinBarImg;
    [SerializeField] Sprite WinStatusBG;
    [SerializeField] Sprite LoseBarImg;
    [SerializeField] Sprite LostStatusBG;

    private bool _gainReward = false;
    private bool _inRoll = false;
    private float _palstance = 270;
    private float _roteAngle = 0;
    private float _waitAddCoins = 0;
    private float _MaxDPSBarSize = 700;

    private int[] _mutiples = new int[] { 6, 5, 4, 2, 4, 3, 6, 5, 4, 2, 3, 2 };
    private int indexSpan;
    private float muiltRewardConis;
    private bool rewardByVideo;

    private void Update()
    {
        if (!_inRoll)
        {
            return;
        }

        var r = _palstance * Time.deltaTime;
        _roteAngle += r;
        Pointer.transform.RotateAround(WheelCenter.position, Vector3.forward, r);

        var tmpAnle = (_roteAngle + indexSpan / 2) % 360;
        muiltRewardConis = _mutiples[(int)(tmpAnle / indexSpan)] * _waitAddCoins;
        RewardCoinsText.text = TextUtils.GetCoinsStringWithUnitAndInt(muiltRewardConis);
    }

    private void Awake()
    {
        indexSpan = 360 / _mutiples.Length;
    }

    public void Hide(Action OnClose = null)
    {
        if (_gainReward)
        {
            return;
        }
        _gainReward = true;

        _inRoll = false;
        _roteAngle = 0;

        _waitAddCoins = rewardByVideo ? muiltRewardConis : _waitAddCoins;
        rewardByVideo = false;

        AudioManager.Instance.StopWheel();
        AudioManager.Instance.PlayGetCoins();

        GameManagers.Instance.PlayerRecored.Coins += _waitAddCoins;
        GameManagers.Instance.PlayerRecored.SaveToLocal();

        StartCoroutine(DelayWork(0.5f, () =>
        {
            FinishPanel.SetActive(false);
            OnClose?.Invoke();
        }));
    }

    public void Show(bool win, float waitAddCoins)
    {
        _gainReward = false;
        rewardByVideo = false;
        _waitAddCoins = waitAddCoins;
        if (win)
        {
            StatusBar.sprite = GameObject.Instantiate(WinBarImg);
            StatusBG.sprite = GameObject.Instantiate(WinStatusBG);
        }
        else
        {
            StatusBar.sprite = GameObject.Instantiate(LoseBarImg);
            StatusBG.sprite = GameObject.Instantiate(LostStatusBG);
        }

        FinishPanel.SetActive(true);
        ClearUIAnimator.SetTrigger("Show");
        ChangeGainCoinsShow(waitAddCoins);
        var sizeDate = TotalDpsBar.rectTransform.sizeDelta;
        float percent = 1.0f - EvilManager.Instance.BlooadPercent;
        TotalDpsBar.rectTransform.sizeDelta = new Vector2(percent * _MaxDPSBarSize, sizeDate.y);
        TotalDpsText.text = (percent * 100).ToString("0") + "%";

        StartCoroutine(DelayWork(1.5f, () =>
        {
            _inRoll = true;
            AudioManager.Instance.PlayWheel();
        }));
    }

    private IEnumerator DelayWork(float deley, Action work)
    {
        yield return new WaitForSeconds(deley);

        work.Invoke();
    }

    public void OnRewardClick()
    {
        Ads.Instance.report("RewardCoins");
        AudioManager.Instance.PlayClick();
        AudioManager.Instance.StopWheel();
        _inRoll = false;

        Ads.Instance.ShowRV((theReward) =>
        {
            rewardByVideo = theReward;
            UIManager.Instance.OnNextClick();
        });
    }

    private void ChangeGainCoinsShow(double coins)
    {
        PlayerConinsTextInFinishUI.text = TextUtils.GetCoinStringWithUnit(GameManagers.Instance.PlayerRecored.Coins);
        GainConisText.text = "+" + TextUtils.GetCoinStringWithUnit(coins);
    }
}

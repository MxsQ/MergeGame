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

    public void Hide()
    {
        FinishPanel.SetActive(false);
    }

    public void Show(bool win, float waitAddCoins)
    {
        FinishPanel.SetActive(true);
        ClearUIAnimator.SetTrigger("Show");
        ChangeGainCoinsShow(waitAddCoins);
    }

    private void ChangeGainCoinsShow(double coins)
    {
        PlayerConinsTextInFinishUI.text = TextUtils.GetCoinStringWithUnit(GameManagers.Instance.PlayerRecored.Coins);
        GainConisText.text = "+" + TextUtils.GetCoinStringWithUnit(coins);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUtils
{
    private static string[] _coinUnit = new string[] { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };

    public static string GetCoinStringWithUnit(double coin)
    {
        double showNum = coin;
        var unitIndex = 0;
        while (showNum > 1000)
        {
            showNum /= 1000;
            unitIndex++;
        }

        if (unitIndex == 0)
        {
            return showNum.ToString();
        }
        else if (showNum * 10 % 10 == 0)
        {
            return showNum.ToString("0") + " " + _coinUnit[unitIndex];
        }

        return showNum.ToString("0.0") + " " + _coinUnit[unitIndex];
    }

    public static string GetCoinsStringWithUnitAndInt(double coin)
    {
        double showNum = coin;
        var unitIndex = 0;
        while (showNum > 1000)
        {
            showNum /= 1000;
            unitIndex++;
        }

        if (unitIndex == 0)
        {
            return showNum.ToString();
        }

        return showNum.ToString("0") + " " + _coinUnit[unitIndex];
    }

}

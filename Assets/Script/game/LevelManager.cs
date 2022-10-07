using System.Collections;
using UnityEngine;


public class LevelManager
{

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get { return _instance; }
    }

    public static void CreateManager()
    {
        _instance = new LevelManager();
    }

    public GameData GameData;

    private LevelManager()
    {
        GameData = DataParser.ParseFromJson();
        GameManagers.OnGameWin += OnGameWin;

        Debug.Log("coins:" + GameData.LevelInfo[1].coins.ToString());
    }

    public int GetLevelHP()
    {
        return GameData.LevelInfo[GameManagers.Instance.PlayerRecored.Level].HP;
    }

    private void OnGameWin()
    {
        var record = GameManagers.Instance.PlayerRecored;
        record.Level += 1;
    }

    public float GetLevelCoins(int level)
    {
        return GameData.LevelInfo[level].coins;
    }

    // @count what total count now is.
    public float GetRolePriceBy(int count)
    {
        if (count == 0)
        {
            return 0;
        }
        else if (count == 1)
        {
            return 100;
        }
        else
        {
            return Mathf.Round(100 * Mathf.Pow(1.1f, count - 1));
        }
    }
}

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
}

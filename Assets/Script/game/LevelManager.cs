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
    private int _curLevel = 1;

    private LevelManager()
    {
        GameData = DataParser.ParseFromJson();
    }
}

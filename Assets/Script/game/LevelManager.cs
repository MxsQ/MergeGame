using System.Collections;
using UnityEngine;


public class LevelManager : MonoBehaviour
{

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get { return _instance; }
    }

    public GameData GameData;

    private int _curLevel = 1;

    private void Awake()
    {
        _instance = this;
        GameData = DataParser.ParseFromJson();
        DontDestroyOnLoad(this);
    }

    private void syncCurrentLevel()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager
{

    private const int CAPTAIN_SPAN = 5;
    private const int BOSS_SPAN = 10;

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
    private LevelFactor _curLevelFactor = new LevelFactor();

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
        var before = record.Level / 10;
        record.Level += 1;
        var after = record.Level / 10;

        if (after > before)
        {
            GameManagers.Instance.BuildNextSeriesEvil(after);
        }
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

    public List<EnemyUnit> GetLevelEvils()
    {
        var gameManager = GameManagers.Instance;

        var level = gameManager.PlayerRecored.Level - 1;
        int configIndex = level / 10;
        int underLevel = level % 10;

        string[] esInfo = gameManager.LevelsConfigs[configIndex].Evils[underLevel].Split('-');
        List<EnemyUnit> es = new List<EnemyUnit>();

        bool hasArcher = false;
        bool hasWarrior = false;

        foreach (string k in esInfo)
        {
            Debug.Log("load " + k);
            var evil = gameManager.GetEvil(k);
            EnemyUnit unit = new EnemyUnit();
            unit.o = evil;


            var scale = level % BOSS_SPAN == 0
                ? gameManager.Config.EvilMaxBoxRadius
                : gameManager.Config.EvilMinBoxRadius;

            evil.transform.localScale = new Vector3(scale, scale, 1);
            evil.transform.localEulerAngles = new Vector3(0, -180, 0);

            if (k.Contains("A"))
            {
                hasArcher = true;
                unit.type = HeroConstance.ARCHER;
            }
            else
            {
                hasWarrior = true;
                unit.type = HeroConstance.WORRIOR;
            }

            es.Add(unit);
            Debug.Log("now evil key = " + k);
        }

        Debug.Log("hasArcher:" + hasArcher.ToString() + " hasWarriro:" + hasWarrior.ToString());

        // config HP and ATk factor
        _curLevelFactor = new LevelFactor();
        if (es.Count > 1)
        {
            if (hasArcher && hasWarrior)
            {
                _curLevelFactor.EvilWarriorHPFactor = gameManager.Config.EvilWarriorHPFacotr;
                _curLevelFactor.EvilWarriorATKFactor = gameManager.Config.EvilWarriorATKFactor;
                _curLevelFactor.EvilArcherHPFactor = gameManager.Config.EvilArcherHPFactor;
                _curLevelFactor.EvilWarriorATKFactor = gameManager.Config.EvilArcherATKFactor;
            }
            else if (hasWarrior)
            {
                _curLevelFactor.EvilWarriorHPFactor = 0.5f;
                _curLevelFactor.EvilWarriorATKFactor = gameManager.Config.EvilWarriorATKFactor;
            }
            else
            {
                _curLevelFactor.EvilArcherHPFactor = 0.5f;
                _curLevelFactor.EvilWarriorATKFactor = gameManager.Config.EvilArcherATKFactor;
            }
        }


        return es;
    }

    public LevelFactor LevelFactor
    {
        get { return _curLevelFactor; }
    }

}


public class LevelFactor
{
    public float EvilWarriorHPFactor = 1;
    public float EvilWarriorATKFactor = 1;

    public float EvilArcherHPFactor = 1;
    public float EvilArcherATKFactor = 1;
}

public class EnemyUnit
{
    public GameObject o;
    public int type;
}

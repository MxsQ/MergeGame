using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
//using ExcelDataReader;

public class DataParser
{
    //private string VALUE_PATH = Application.dataPath + "/Resources/excel/Value.xlsx";

    public static readonly int MAX_GAME_LEVEL = 12;
    public static readonly int MAX_ROLE_LEVEL = 8;
    public static readonly int EVIL_HP_INDEX = 1;
    public static readonly int EVIL_ATK_INDEX = 2;
    public static readonly int WARRIOR_LEVEL_INDEX = 3;
    public static readonly int WARRIOR_HP_INDEX = 4;
    public static readonly int WARRIOR_ATK_INDEX = 5;
    public static readonly int ARCHER_HP_INDEX = 6;
    public static readonly int ARCHER_ATK_INDEX = 7;

    public static void ParseFromExcel()
    {
        var excelPath = Application.dataPath + "/Resources/excel/Value.xlsx";

        FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
        Excel.IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        var a = result.Tables[0];
        GameData gameData = new GameData();

        Debug.Log(a.Columns.Count);
        Debug.Log(a.Rows.Count);

        for (int i = 1; i < MAX_GAME_LEVEL; i++)
        {
            Debug.Log(a.Rows[i][0]);
        }



        //for (int i = 1; i <= MAX_GAME_LEVEL + 1; i++)
        //{
        //    LevelData data = new LevelData();
        //    data.ATK = (int)result.Tables[0].Rows[i][EVIL_ATK_INDEX];
        //    data.HP = (int)result.Tables[0].Rows[i][EVIL_HP_INDEX];
        //    gameData.LevelInfo[i] = data;
        //}

        //for (int i = 1; i <= MAX_ROLE_LEVEL; i++)
        //{
        //    WarriorData wData = new WarriorData();
        //    wData.ATK = (int)result.Tables[0].Rows[i][WARRIOR_ATK_INDEX];
        //    wData.HP = (int)result.Tables[0].Rows[i][WARRIOR_HP_INDEX];

        //    ArcherData aData = new ArcherData();
        //    aData.ATK = (int)result.Tables[0].Rows[i][ARCHER_ATK_INDEX];
        //    aData.HP = (int)result.Tables[0].Rows[i][ARCHER_HP_INDEX];

        //    gameData.ArcherInfo[i] = aData;
        //    gameData.WarriorInfo[i] = wData;
        //}

        //GameData.Instance = gameData;
        //gameData.ToString();
    }

    public static GameData ParseFromJson()
    {
        //var jsonPath = Application.dataPath + "/Resources/json/Value.json";
        //var jsonString = File.ReadAllText(jsonPath, Encoding.UTF8);

        TextAsset ta = Resources.Load<TextAsset>("json/Value");
        byte[] rb = Encoding.UTF8.GetBytes(ta.text);
        GameJson data = JsonUtility.FromJson<GameJson>(UTF8Encoding.UTF8.GetString(rb));

        GameData gameData = new GameData();

        for (int i = 0; i <= MAX_ROLE_LEVEL; i++)
        {
            gameData.WarriorInfo[data.warrior[i].level] = data.warrior[i];
            gameData.ArcherInfo[data.archer[i].level] = data.archer[i];
        }

        for (int i = 0; i < MAX_GAME_LEVEL; i++)
        {
            gameData.LevelInfo[data.game[i].level] = data.game[i];
        }

        gameData.ToString();
        return gameData;

    }

    [Serializable]
    public class LevelData
    {
        public int level;
        public int HP;
        public int ATK;
        public float coins;
    }

    [Serializable]
    public class WarriorData
    {
        public int level;
        public int HP;
        public int ATK;
    }

    [Serializable]
    public class ArcherData
    {
        public int level;
        public int HP;
        public int ATK;
    }
    [Serializable]
    public class GameJson
    {
        public LevelData[] game;
        public WarriorData[] warrior;
        public ArcherData[] archer;
    }
}

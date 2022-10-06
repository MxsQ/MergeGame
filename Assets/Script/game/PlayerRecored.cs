using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class PlayerRecored
{
    private static string RECORD = "record";

    public int Level = 1;
    public int Coins;
    public Dictionary<int, LayInfo> LayoutInfos = new Dictionary<int, LayInfo>();

    public string LayJson;

    private PlayerRecored() { }

    public void Record(int index, int level, int type)
    {
        LayInfo info = new LayInfo();
        info.Index = index;
        info.Level = level;
        info.Type = type;
        if (LayoutInfos.ContainsKey(index))
        {
            LayoutInfos.Remove(index);
        }
        LayoutInfos.Add(index, info);
    }

    public void ClearLayInfo()
    {
        LayoutInfos.Clear();
    }

    public void ClearCache()
    {
        PlayerPrefs.SetString(RECORD, "");
    }


    public void SaveToLocal()
    {
        LayJson = JsonConvert.SerializeObject(LayoutInfos);
        string json = JsonUtility.ToJson(this);
        //Debug.Log("cur json :" + json);
        PlayerPrefs.SetString(RECORD, json);
    }

    public static PlayerRecored GetFromLocal()
    {
        string json = PlayerPrefs.GetString(RECORD, "");
        Debug.Log("local json: " + json);
        if (json.Length <= 0)
        {
            return new PlayerRecored();
        }

        PlayerRecored playerRecord = JsonUtility.FromJson<PlayerRecored>(json);
        playerRecord.LayoutInfos = JsonConvert.DeserializeObject<Dictionary<int, LayInfo>>(playerRecord.LayJson);
        return playerRecord;
    }
}

[Serializable]
public class LayInfo
{
    public int Index;
    public int Level;
    public int Type;
}

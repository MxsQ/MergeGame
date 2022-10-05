using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecored
{
    public int Level = 1;
    public int Coins;
    public Dictionary<int, LayInfo> LayoutInfos = new Dictionary<int, LayInfo>();

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

}

public class LayInfo
{
    public int Index;
    public int Level;
    public int Type;
}

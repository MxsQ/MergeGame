using UnityEditor;
using UnityEngine;
using static DataParser;

public class GameData
{

    public static float EvilWarriorHPRate = 0.75f;
    public static float EvilWarriorATKRate = 0.3f;

    public WarriorData[] WarriorInfo = new WarriorData[DataParser.MAX_ROLE_LEVEL + 1];
    public ArcherData[] ArcherInfo = new ArcherData[DataParser.MAX_ROLE_LEVEL + 1];
    public LevelData[] LevelInfo = new LevelData[DataParser.MAX_GAME_LEVEL + 1];


    public void ToString()
    {
        Debug.Log("level info:" + LevelInfo);
        Debug.Log("archer info:" + ArcherInfo);
        Debug.Log("warriro info:" + WarriorInfo);
    }

    public RoleData GetEvilWarriorData(int level)
    {
        RoleData roleData = new RoleData();
        var factors = LevelManager.Instance.LevelFactor;
        roleData.HP = (int)(LevelInfo[level].HP * factors.EvilWarriorHPFactor);
        roleData.ATK = (int)(LevelInfo[level].ATK * factors.EvilWarriorATKFactor);

        return roleData;
    }

    public RoleData GetEvilArcherData(int level)
    {
        RoleData roleData = new RoleData();
        var factors = LevelManager.Instance.LevelFactor;
        roleData.HP = (int)(LevelInfo[level].HP * factors.EvilArcherHPFactor);
        roleData.ATK = (int)(LevelInfo[level].ATK * factors.EvilArcherATKFactor);

        return roleData;
    }

    public RoleData GetWarriorData(int level)
    {
        RoleData roleData = new RoleData();
        roleData.HP = WarriorInfo[level].HP;
        roleData.ATK = WarriorInfo[level].ATK;

        return roleData;
    }

    public RoleData GetArcherData(int level)
    {
        RoleData roleData = new RoleData();
        roleData.HP = ArcherInfo[level].HP;
        roleData.ATK = ArcherInfo[level].ATK;

        return roleData;
    }
}

public class RoleData
{
    public int HP;
    public int ATK;
}

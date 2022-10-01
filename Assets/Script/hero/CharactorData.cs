using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorData
{
    public GameObject Character;
    public int Type;
    public int Level;

    public CharactorData(GameObject character, int type, int level = 0)
    {
        Character = character;
        Type = type;
        Level = level;
    }

    public bool IsSame(CharactorData cmp)
    {
        return cmp != null && Type == cmp.Type && Level == cmp.Level;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "level", menuName = "Level/Config")]
public class LevelScriptableObject : ScriptableObject
{
    public string Dir;

    public List<string> Evils;
}

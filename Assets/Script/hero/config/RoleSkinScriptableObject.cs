using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "character", menuName = "Character/SkinConfig")]
public class RoleSkinScriptableObject : ScriptableObject
{
    public string Directory;
    public List<string> Items;
}
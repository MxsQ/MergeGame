using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "character", menuName = "Character/Config")]
public class CharacterScriptableObject : ScriptableObject
{
    public GameObject[] characters;
}

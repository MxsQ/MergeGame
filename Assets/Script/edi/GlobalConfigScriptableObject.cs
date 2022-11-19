using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "character", menuName = "Character/GlobalConfig")]
public class GlobalConfigScriptableObject : ScriptableObject
{
    [Header("War Widget")]
    public GameObject Arrow;
    public GameObject HitText;

    [Header("War Property")]
    public float ArrowSizeScale;
    public float ArrowSpeedFactory;
    public float ArrowSpeed;
    public float MoveSpeed;


    [Header("Outlock")]
    public int HeroBoxRadius;
    public int EvilMinBoxRadius;
    public int EvilMaxBoxRadius;

    [Header("Fight")]
    public int HeroCollideRadius = 90;
    public int EvilCollideRadius = 120;
    public float EvilWarriorHPFacotr;
    public float EvilWarriorATKFactor;
    public float EvilArcherHPFactor;
    public float EvilArcherATKFactor;
}

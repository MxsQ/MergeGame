using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "character", menuName = "Character/GlobalConfig")]
public class GlobalConfigScriptableObject : ScriptableObject
{
    public GameObject Arrow;
    public GameObject HitText;

    public float ArrowSizeScale;
    public float ArrowSpeedFactory;
    public float ArrowSpeed;
    public float MoveSpeed;

    public int HeroBoxRadius;

    public int EvilMinBoxRadius;
    public int EvilMaxBoxRadius;

    public float EvilWarriorHPFacotr;
    public float EvilWarriorATKFactor;
    public float EvilArcherHPFactor;
    public float EvilArcherATKFactor;
}

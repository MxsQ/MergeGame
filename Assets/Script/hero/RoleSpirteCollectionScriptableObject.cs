using HeroEditor.Common;
using UnityEditor;
using UnityEngine;


/**
 * 
 * 从JSON生成角色的时候，SpriteCollection的来源不一样，导致找不到对应的Sprite，故采用编辑器里边配置的
 */
[CreateAssetMenu(fileName = "character", menuName = "Character/SriteCollectionConfig")]
public class RoleSpirteCollectionScriptableObject : ScriptableObject
{
    [SerializeField] public SpriteCollection MegapackCollection;

    [SerializeField] public SpriteCollection FantasyCollection;
}

using Assets.HeroEditor.Common.CharacterScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    private static GameManagers _instance = null;

    [SerializeField] public GlobalConfigScriptableObject Config;

    [SerializeField] public GameObject Target;

    private List<GameObject> _hero = new List<GameObject>();
    private List<GameObject> _enmey = new List<GameObject>();

    public static GameManagers Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    public GameObject FindEnemy(GameObject referencePs)
    {
        //return FindClosedTart(_enmey, referencePs);
        return Target;
    }

    public GameObject FindHero(GameObject referencePs)
    {
        return FindClosedTart(_enmey, referencePs);
    }

    private GameObject FindClosedTart(List<GameObject> candidate, GameObject referencePs)
    {

        GameObject target = null;
        float distance = int.MaxValue;
        float dis;
        foreach (GameObject t in candidate)
        {
            dis = Vector3.Distance(referencePs.transform.position, t.transform.position);
            if (dis < distance)
            {
                distance = dis;
                target = t;
            }
        }

        return target;
    }

    public void RegisterHero(GameObject hero) => _hero.Add(hero);
    public void UnRegisterHero(GameObject hero) => _hero.Remove(hero);
    public void RegisterEnemy(GameObject enemy) => _enmey.Add(enemy);
    public void UnRegisterEnemy(GameObject enemy) => _enmey.Remove(enemy);

    internal GameObject FindEnemy(Character character)
    {
        throw new NotImplementedException();
    }
}

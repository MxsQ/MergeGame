using Assets.HeroEditor.Common.CharacterScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagers : MonoBehaviour
{
    private static GameManagers _instance = null;

    [SerializeField] public GlobalConfigScriptableObject Config;

    [SerializeField] public RoleSpirteCollectionScriptableObject RoleSpirteCollection;

    [SerializeField] public RoleSkinScriptableObject WarriroSkinConfig;

    [SerializeField] public GameObject RoleCopyHost;

    private Dictionary<RoleSkin, List<GameObject>> _roles = new Dictionary<RoleSkin, List<GameObject>>();
    private RoleSkin _curSkin = RoleSkin.WARRIOR_DEFAUL;

    private List<GameObject> _hero = new List<GameObject>();
    private List<GameObject> _enmey = new List<GameObject>();

    public static event Action OnGameStart;
    public static event Action OnGameEnd;


    public static bool InGame;


    public static GameManagers Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
        BuildCharacter(_curSkin, WarriroSkinConfig);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public GameObject FindEnemy(GameObject referencePs)
    {
        return FindClosedTart(_enmey, referencePs);
        //return Target;
    }

    public GameObject FindHero(GameObject referencePs)
    {
        return FindClosedTart(_hero, referencePs);
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

    private void BuildCharacter(RoleSkin skinType, RoleSkinScriptableObject skinConfig)
    {
        if (_roles.ContainsKey(skinType))
        {
            return;
        }


        List<GameObject> roles = new List<GameObject>();
        for (int i = 0; i <= DataParser.MAX_ROLE_LEVEL; i++)
        {
            roles.Add(Instantiate(RoleCopyHost));
        }

        var dir = Application.dataPath + "/Resources/role/" + skinConfig.Directory + "/";
        for (int i = 0; i <= DataParser.MAX_ROLE_LEVEL; i++)
        {
            roles[i].gameObject.transform.position = new Vector3(10, 10, 0);
            Character c = roles[i].GetComponent<Character>();
            try
            {
                c.SpriteCollection = Instantiate(RoleSpirteCollection.MegapackCollection);
                var roleJsonString = System.IO.File.ReadAllText(dir + skinConfig.Items[i] + ".json");
                c.FromJson(roleJsonString);
            }
            catch (Exception e)
            {
                c.SpriteCollection = Instantiate(RoleSpirteCollection.FantasyCollection);
                var roleJsonString = System.IO.File.ReadAllText(dir + skinConfig.Items[i] + ".json");
                c.FromJson(roleJsonString);
            }
            DontDestroyOnLoad(roles[i]);
        }

        _roles.Add(skinType, roles);
    }

    public GameObject GetWarriorCharacter(int level)
    {
        return Instantiate(_roles[_curSkin][level]);
    }

    public void InvodeGameStart()
    {
        InGame = true;
        OnGameStart.Invoke();
    }

    public void InvodeGameEnd()
    {
        InGame = false;
        OnGameEnd.Invoke();
    }
}

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

    [SerializeField] public GameObject RoleCopyReference;

    [SerializeField] public GameObject RoleHost;

    private Dictionary<RoleSkin, List<GameObject>> _roles = new Dictionary<RoleSkin, List<GameObject>>();
    private RoleSkin _curSkin = RoleSkin.WARRIOR_DEFAUL;

    private List<Role> _hero = new List<Role>();
    private List<Role> _enmey = new List<Role>();

    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    public static event Action OnGameWin;
    public static event Action OnGameFaild;
    public static event Action<int> OnLevelChange;


    public static bool InGame;
    private bool _warFinish;

    public PlayerRecored PlayerRecored;


    public static GameManagers Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
        PlayerRecored = PlayerRecored.GetFromLocal();
    }

    private void OnValidate()
    {
        BuildCharacter(_curSkin, WarriroSkinConfig);
        LevelManager.CreateManager();
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public Role FindEnemy(GameObject referencePs)
    {
        return FindClosedTart(_enmey, referencePs);
        //return Target;
    }

    public Role FindHero(GameObject referencePs)
    {
        return FindClosedTart(_hero, referencePs);
    }

    private Role FindClosedTart(List<Role> candidate, GameObject referencePs)
    {

        Role target = null;
        float distance = int.MaxValue;
        float dis;
        foreach (Role t in candidate)
        {
            dis = Vector3.Distance(referencePs.transform.position, t.Position());
            if (dis < distance)
            {
                distance = dis;
                target = t;
            }
        }

        return target;
    }

    public void RegisterHero(Role hero) => _hero.Add(hero);
    public void UnRegisterHero(Role hero) => _hero.Remove(hero);
    public void RegisterEnemy(Role enemy) => _enmey.Add(enemy);
    public void UnRegisterEnemy(Role enemy) => _enmey.Remove(enemy);

    private void BuildCharacter(RoleSkin skinType, RoleSkinScriptableObject skinConfig)
    {
        if (_roles.ContainsKey(skinType))
        {
            return;
        }


        List<GameObject> roles = new List<GameObject>();
        for (int i = 0; i <= DataParser.MAX_ROLE_LEVEL; i++)
        {
            roles.Add(Instantiate(RoleCopyReference));
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
            roles[i].gameObject.transform.SetParent(RoleHost.transform);
            DontDestroyOnLoad(roles[i]);
        }

        _roles.Add(skinType, roles);
    }

    public GameObject GetWarriorCharacter(int level)
    {
        return Instantiate(_roles[_curSkin][level]);
    }

    public void InvokeGameStart()
    {
        _warFinish = false;
        InGame = true;
        OnGameStart.Invoke();
    }

    public void InvokeGameEnd()
    {
        InGame = false;
        _enmey.Clear();
        _hero.Clear();
        OnGameEnd.Invoke();
        InvokeLevelChange();
    }

    public void InvokeGameWin()
    {
        Debug.Log("win.");
        OnGameWin.Invoke();
    }

    public void InvokeGameFaild()
    {
        Debug.Log("Faild.");
        OnGameFaild.Invoke();
    }

    public void InvokeLevelChange()
    {
        OnLevelChange.Invoke(PlayerRecored.Level);
    }

    public void OnHeroDeath(Role role)
    {
        _hero.Remove(role);

        if (_warFinish)
        {
            return;
        }
        if (_hero.Count <= 0)
        {
            InvokeGameFaild();
            _warFinish = true;
            //InvodeGameEnd();
        }
    }

    public void OnEvilDeath(Role role)
    {
        _enmey.Remove(role);
        if (_warFinish)
        {
            return;
        }
        if (_enmey.Count <= 0)
        {
            InvokeGameWin();
            _warFinish = true;
            //InvodeGameEnd();
        }
    }
}

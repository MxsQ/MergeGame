using Assets.HeroEditor.Common.CharacterScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagers : MonoBehaviour
{
    private static GameManagers _instance = null;

    [SerializeField] public GlobalConfigScriptableObject Config;

    [SerializeField] public RoleSpirteCollectionScriptableObject RoleSpirteCollection;

    [SerializeField] public RoleSkinScriptableObject[] WarriorSkin;

    [SerializeField] public RoleSkinScriptableObject[] ArcherSkin;

    [SerializeField] public GameObject RoleCopyReference;

    [SerializeField] public GameObject RoleHost;

    [SerializeField] public LevelScriptableObject[] LevelsConfigs;

    [SerializeField] public Sprite[] LevelBG;

    private Dictionary<RoleSkin, List<GameObject>> _roles = new Dictionary<RoleSkin, List<GameObject>>();
    private RoleSkin _curWarriorSkin = RoleSkin.WARRIOR_DEFAUL;
    private RoleSkin _curArcherSkin = RoleSkin.ARCHER_DEFAUL;

    private List<Role> _hero = new List<Role>();
    private List<Role> _enmey = new List<Role>();

    private Dictionary<string, GameObject> _evils = new Dictionary<string, GameObject>();

    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    public static event Action OnGameWin;
    public static event Action OnGameFaild;
    public static event Action<int> OnLevelChange;
    public static event Action<RoleSkin> OnSkinChange;


    public static bool InGame;
    private bool _warFinish;

    private string _resouseDir = "";

    public PlayerRecored PlayerRecored;

    public static GameManagers Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        //Application.targetFrameRate = 2;
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        FullApplicationConfig();

        PlayerRecored = PlayerRecored.GetFromLocal();
        LevelManager.CreateManager();

        BuildHero(RoleSkin.WARRIOR_DEFAUL, WarriorSkin[0]);
        BuildHero(RoleSkin.WARRIOR_1, WarriorSkin[1]);
        BuildHero(RoleSkin.WARRIOR_2, WarriorSkin[2]);
        BuildHero(RoleSkin.WARRIOR_3, WarriorSkin[3]);
        BuildHero(RoleSkin.ARCHER_DEFAUL, ArcherSkin[0]);
        BuildHero(RoleSkin.ARCHER_1, ArcherSkin[1]);
        BuildEvil(LevelsConfigs[PlayerRecored.Level / 10]);

        _curWarriorSkin = SkinManager.Skins[PlayerRecored.WarriorSkinIndex];
        _curArcherSkin = SkinManager.Skins[PlayerRecored.ArcherSkinIndex];
        //StartCoroutine(DeleyWork(5, () => SceneManager.LoadScene("GameScene", LoadSceneMode.Additive)));
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

    }

    private IEnumerator DeleyWork(float delay, Action work)
    {
        yield return new WaitForSeconds(delay);

        work?.Invoke();
    }

    private void FullApplicationConfig()
    {
        //#if UNITY_ANDROID
        //        //_resouseDir = Application.streamingAssetsPath;
        //        _resouseDir = "jar:file://" + Application.dataPath + "!/assets";
        //#endif

        //#if UNITY_EDITOR
        //        _resouseDir = Application.dataPath + "/Resources";
        //#endif
        _resouseDir = Application.streamingAssetsPath;
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

    private void BuildHero(RoleSkin skinType, RoleSkinScriptableObject skinConfig)
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

        //var dir = _resouseDir + "/role/" + skinConfig.Directory + "/";
        var dir = "role/" + skinConfig.Directory + "/";

        for (int i = 0; i <= DataParser.MAX_ROLE_LEVEL; i++)
        {
            roles[i].gameObject.transform.position = new Vector3(10, 10, 0);
            Character c = roles[i].GetComponent<Character>();
            //fullCharacter(c, dir + skinConfig.Items[i] + ".json");
            fullCharacter(c, dir + skinConfig.Items[i]);
            roles[i].gameObject.transform.SetParent(RoleHost.transform);
            roles[i].gameObject.SetActive(false);
            DontDestroyOnLoad(roles[i]);
        }

        _roles.Add(skinType, roles);
    }

    public void BuildNextSeriesEvil(int seriesIndex)
    {
        BuildEvil(LevelsConfigs[seriesIndex]);
    }

    private void BuildEvil(LevelScriptableObject levelConfig)
    {
        //var dir = _resouseDir + "/role/" + levelConfig.Dir + "/";

        var dir = "role/" + levelConfig.Dir + "/";

        Debug.Log("build evile from " + dir);
        for (int i = 0; i < HeroConstance.LEVEL_SERIES_SIZE; i++)
        {
            string[] es = levelConfig.Evils[i].Split('-');
            for (int j = 0; j < es.Length; j++)
            {
                var key = es[j];
                if (_evils.ContainsKey(key))
                {
                    continue;
                }
                var evil = Instantiate(RoleCopyReference);
                evil.gameObject.transform.position = new Vector3(10, 10, 0);
                evil.gameObject.transform.SetParent(RoleHost.transform);
                evil.gameObject.SetActive(false);
                _evils.Add(key, evil);
                Character c = evil.GetComponent<Character>();
                //fullCharacter(c, dir + key + ".json");
                fullCharacter(c, dir + key);

                DontDestroyOnLoad(evil);
            }

        }

    }

    private void fullCharacter(Character character, string filePath)
    {
        //var roleJsonString = System.IO.File.ReadAllText(filePath);
        //try
        //{
        //    character.SpriteCollection = Instantiate(RoleSpirteCollection.MegapackCollection);
        //    character.FromJson(roleJsonString);
        //}
        //catch (Exception e)
        //{
        //    character.SpriteCollection = Instantiate(RoleSpirteCollection.FantasyCollection);
        //    character.FromJson(roleJsonString);
        //}

        TextAsset ta = Resources.Load<TextAsset>(filePath);
        byte[] rb = Encoding.UTF8.GetBytes(ta.text);
        var roleJsonString = UTF8Encoding.UTF8.GetString(rb);
        try
        {
            character.SpriteCollection = Instantiate(RoleSpirteCollection.MegapackCollection);
            character.FromJson(roleJsonString);
        }
        catch (Exception e)
        {
            character.SpriteCollection = Instantiate(RoleSpirteCollection.FantasyCollection);
            character.FromJson(roleJsonString);
        }
    }

    public GameObject GetWarriorCharacter(int level)
    {
        var c = Instantiate(_roles[_curWarriorSkin][level]);
        c.gameObject.SetActive(true);
        return c;
    }

    public GameObject GetWarriorCharacter(int level, RoleSkin skin)
    {
        var c = Instantiate(_roles[skin][level]);
        c.gameObject.SetActive(true);
        return c;
    }

    public GameObject GetArcherCharacter(int level)
    {
        var c = Instantiate(_roles[_curArcherSkin][level]);
        c.gameObject.SetActive(true);
        return c;
    }

    public GameObject GetArcherCharacter(int level, RoleSkin skin)
    {
        var c = Instantiate(_roles[skin][level]);
        c.gameObject.SetActive(true);
        return c;
    }

    public GameObject GetEvil(string key)
    {
        var c = Instantiate(_evils[key]);
        c.gameObject.SetActive(true);
        return c;
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
        PlayerRecored.SaveToLocal();
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

    public void InvokeSkinChange(RoleSkin roleSkin)
    {
        var skinIndex = SkinManager.Instance.SkinIndex;
        if (roleSkin == RoleSkin.ARCHER_1 || roleSkin == RoleSkin.ARCHER_DEFAUL)
        {
            _curArcherSkin = roleSkin;
            PlayerRecored.ArcherSkinIndex = skinIndex;
        }
        else
        {
            _curWarriorSkin = roleSkin;
            PlayerRecored.WarriorSkinIndex = skinIndex;
        }
        PlayerRecored.SaveToLocal();
        OnSkinChange.Invoke(roleSkin);
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

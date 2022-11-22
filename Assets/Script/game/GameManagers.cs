using Assets.HeroEditor.Common.CharacterScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SkinManager;

public class GameManagers : MonoBehaviour
{
    private static GameManagers _instance = null;

    [SerializeField] public GlobalConfigScriptableObject Config;

    [SerializeField] public RoleSpirteCollectionScriptableObject RoleSpirteCollection;


    [SerializeField] public GameObject RoleCopyReference;

    [SerializeField] public GameObject RoleHost;

    [SerializeField] public LevelScriptableObject[] LevelsConfigs;

    [SerializeField] public Sprite[] LevelBG;

    [SerializeField] public Skin[] HeroSkin;

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
        //LevelManager.CreateManager();

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

    public Skin GetSkin(RoleSkin skin)
    {
        Skin target = null;
        foreach (Skin s in HeroSkin)
        {
            if (s.ID == skin)
            {
                target = s;
            }
        }
        return target;
    }


    public GameObject GetWarriorCharacter(int level)
    {
        return GetWarriorCharacter(level, _curWarriorSkin);
    }

    public GameObject GetWarriorCharacter(int level, RoleSkin skin)
    {
        GameObject warrior = null;
        for (int i = 0; i < HeroSkin.Length; i++)
        {
            if (skin == HeroSkin[i].ID)
            {
                warrior = Instantiate(HeroSkin[i].prefab[level]);
                break;
            }
        }

        warrior.gameObject.SetActive(true);
        return warrior;
    }

    public GameObject GetArcherCharacter(int level)
    {
        return GetArcherCharacter(level, _curArcherSkin);
    }

    public GameObject GetArcherCharacter(int level, RoleSkin skin)
    {
        GameObject archer = null;
        for (int i = 0; i < HeroSkin.Length; i++)
        {
            if (skin == HeroSkin[i].ID)
            {
                archer = Instantiate(HeroSkin[i].prefab[level]);
                break;
            }
        }

        archer.gameObject.SetActive(true);
        return archer;
    }

    public GameObject GetEvil(string key)
    {
        var c = Instantiate(_evils[key]);
        c.gameObject.SetActive(true);
        return c;
    }

    public GameObject GetEvil(GameObject o, float scale)
    {
        if (o == null)
        {
            return null;
        }

        var evilObject = Instantiate(o);
        evilObject.transform.localScale = new Vector3(scale, scale, 1);
        evilObject.transform.localEulerAngles = new Vector3(0, -180, 0);
        evilObject.gameObject.SetActive(true);

        return evilObject;
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



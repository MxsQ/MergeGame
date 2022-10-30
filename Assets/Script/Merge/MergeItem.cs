using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.CommonScripts;
using Assets.HeroEditor.Common.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeItem : MonoBehaviour
{

    [SerializeField] SpriteRenderer EmptyBG;
    [SerializeField] SpriteRenderer OccupyBG;
    [SerializeField] SpriteRenderer Light;

    [SerializeField] AudioSource ATKSource;
    [SerializeField] AudioSource ShootSource;

    private int MAX_CHARACTER_LEVEL = 8;

    private int _length;
    Vector3 _mPostion;
    public bool HasCharesctor { get; set; }
    private CharactorData _data;
    private HeroController _heroController;

    private void Awake()
    {
        HasCharesctor = false;

        _heroController = new HeroController(ShootSource, ATKSource);
        _length = GameManagers.Instance.Config.HeroBoxRadius;
        Log.D("position = " + _mPostion);
        //Log.D("word ps = " + Camera.main.WorldToScreenPoint(_mPostion));

    }

    public void RefereshPosiont()
    {
        _mPostion = gameObject.transform.parent.position;
    }

    public void Update()
    {
        _heroController.Update();
    }

    public void LateUpdate()
    {
        _heroController.LateUpdate();
    }

    public bool InZone(Vector2 screenPs)
    {
        var ps = Camera.main.ScreenToWorldPoint(screenPs);

        if (Mathf.Abs(_mPostion.x - ps.x) <= _length && Mathf.Abs(_mPostion.y - ps.y) <= _length)
        {
            return true;
        }

        return false;
    }

    public void setCharector(CharactorData characterData)
    {


        if (_data != null)
        {
            Destroy(_data.Character);
        }

        _data = characterData;

        HasCharesctor = true;

        _data.Character.transform.localScale = new Vector3(_length * 0.5f, _length * 0.5f, 1);
        _data.Character.transform.parent = gameObject.transform;
        _data.Character.transform.position = new Vector3(_mPostion.x, _mPostion.y - 20, 0);

        _heroController.RetTo(_data);

        ShowOcuppyBG();
    }

    public CharactorData CurCharacterData
    {
        get { return _data; }
    }
    public void MoveFrom(MergeItem exchangeItem)
    {
        var fromData = exchangeItem.CurCharacterData;
        setCharector(new CharactorData(GameObject.Instantiate(fromData.Character), fromData.Type, fromData.Level));
        exchangeItem.Reset();
    }

    public void ExchangeTo(MergeItem exchangeItem)
    {
        var aData = exchangeItem.CurCharacterData;
        var A = GameObject.Instantiate(aData.Character);
        var AType = aData.Type;
        var ALevel = aData.Level;

        var B = GameObject.Instantiate(_data.Character);
        var BType = _data.Type;
        var BLevel = _data.Level;

        exchangeItem.Reset();
        Reset();
        setCharector(new CharactorData(A, AType, ALevel));
        exchangeItem.setCharector(new CharactorData(B, BType, BLevel));
    }

    public bool canbeMerge(MergeItem target)
    {
        if (_data.Character == null
            || _data.Level >= MAX_CHARACTER_LEVEL
            || !HasCharesctor
            || !_data.IsSame(target.CurCharacterData))
        {
            return false;
        }
        return true;
    }

    public bool CanbeExchangePosition(MergeItem targe)
    {
        if (_data.Character == null
            || targe == null
            || !HasCharesctor
            || !targe.HasCharesctor
            || targe.CurCharacterData == null
            || (targe.CurCharacterData.Type == _data.Type && targe.CurCharacterData.Level == _data.Level))
        {
            return false;
        }
        return true;
    }

    public void mergetCharactor(GameObject character)
    {
        setCharector(new CharactorData(character, _data.Type, _data.Level + 1));
    }

    public void MoveTo(Vector2 screenPs)
    {
        var ps = Camera.main.ScreenToWorldPoint(screenPs);

        _data.Character.transform.position = new Vector3(ps.x, ps.y + -_length / 2, 0);
    }

    public void MoveToOriginal()
    {
        _data.Character.transform.position = new Vector3(_mPostion.x, _mPostion.y - 20, 0);
        gameObject.transform.position = _mPostion;
    }

    public GameObject GetCharacter()
    {
        return _data.Character;
    }

    public CharactorData GetCharacterData()
    {
        return _data;
    }

    public void HideAllBG()
    {
        EmptyBG.SetActive(false);
        OccupyBG.SetActive(false);
        Light.SetActive(false);
    }

    public void ShowEmptyBG()
    {
        EmptyBG.SetActive(true);
        OccupyBG.SetActive(false);
        Light.SetActive(false);
    }

    public void ShowOcuppyBG()
    {
        EmptyBG.SetActive(false);
        OccupyBG.SetActive(true);
        Light.SetActive(false);
    }

    public void ShowLight()
    {
        Light.SetActive(true);
    }

    public void HideLight()
    {
        Light.SetActive(false);
    }

    public void Reset()
    {
        HasCharesctor = false;
        MoveToOriginal();
        //_data.Character.transform.parent = null;
        //GameObject.Destroy(_data.Character);
        _heroController.Reset();
        _data = null;

        ShowEmptyBG();
    }

}

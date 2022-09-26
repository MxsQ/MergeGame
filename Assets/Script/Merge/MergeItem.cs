using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeItem : MonoBehaviour
{
    private int MAX_CHARACTER_LEVEL = 8;

    private int _length = 90;
    Vector3 _mPostion;
    public bool HasCharesctor { get; set; }
    private GameObject _character;
    public int CharacterLevel = 0;
    public int CharacterType = -1;
    private HeroController _heroController;

    private void Awake()
    {
        HasCharesctor = false;
        _mPostion = gameObject.transform.position;
        _heroController = new HeroController();
        //Log.D("position = " + _mPostion);
        //Log.D("word ps = " + Camera.main.WorldToScreenPoint(_mPostion));
    }

    public void Update()
    {
        _heroController.Update();
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

    public void setCharector(GameObject character, int characterType, int characterLevel = 0)
    {
        CharacterType = characterType;
        CharacterLevel = characterLevel;
        if (_character != null)
        {
            Destroy(_character);
        }


        _character = character;

        HasCharesctor = true;

        character.transform.localScale = new Vector3(_length * 0.5f, _length * 0.5f, 1);
        character.transform.position = new Vector3(_mPostion.x, _mPostion.y + -_length / 2, 0);
        character.transform.parent = gameObject.transform;

        _heroController.setTo(character, characterType);
    }

    public bool canbeMerge(MergeItem target)
    {
        if (_character == null
            || CharacterLevel >= MAX_CHARACTER_LEVEL
            || HasCharesctor
            || CharacterType != target.CharacterType
            || CharacterLevel != target.CharacterLevel)
        {
            return false;
        }
        return true;
    }

    public void mergetCharactor(GameObject character)
    {
        setCharector(character, _heroController.HeroType, CharacterLevel + 1);
    }

    public void MoveTo(Vector2 screenPs)
    {
        var ps = Camera.main.ScreenToWorldPoint(screenPs);

        _character.transform.position = new Vector3(ps.x, ps.y + -_length / 2, 0);
    }

    public void MoveToOriginal()
    {
        _character.transform.position = new Vector3(_mPostion.x, _mPostion.y + -_length / 2, 0);
    }

    public void Reset()
    {
        HasCharesctor = false;
        _character.transform.parent = null;
        GameObject.Destroy(_character);
        _character = null;
        CharacterLevel = 0;
    }

}

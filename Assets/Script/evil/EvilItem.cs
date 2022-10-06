using System.Collections;
using System.Collections.Generic;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class EvilItem : MonoBehaviour
{
    private Vector3 _originPs;
    private Role _role;
    private GameObject _charecter;

    private void Awake()
    {
        var ps = gameObject.transform.position;
        _originPs = new Vector3(ps.x, ps.y, ps.z);
    }

    public void set(GameObject character)
    {
        _charecter = character;
        _role = new EvilWarrior(character.GetComponent<Character>(), LevelManager.Instance.GameData.GetEvilWarriorData(GameManagers.Instance.PlayerRecored.Level));
        _role?.Register();
    }

    public void Reset()
    {
        if (_charecter != null)
        {
            _charecter.transform.position = new Vector3(_originPs.x, _originPs.y, _originPs.z);
        }
    }

    public void RebuildEviel()
    {
        gameObject.transform.position = _originPs;
        if (_charecter != null)
        {
            Destroy(_charecter);
        }
    }


    private void Update()
    {
        if (!GameManagers.InGame || _role == null)
        {
            return;
        }

        _role.Update();
    }

    public int GetRoleHP()
    {
        if (_role == null)
        {
            return 0;
        }

        return _role.GetCurHP();
    }
}

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

    public void set(EnemyUnit unit)
    {
        var character = unit.o;
        character.transform.parent = gameObject.transform;
        character.transform.localPosition = new Vector3(0, 0, 0);

        _charecter = character;
        if (unit.type == HeroConstance.WORRIOR)
        {
            _role = new EvilWarrior(character.GetComponent<Character>(),
                        LevelManager.Instance.GameData.GetEvilWarriorData(GameManagers.Instance.PlayerRecored.Level));
        }
        else
        {
            _role = new EvilArcher(character.GetComponent<Character>(),
                       LevelManager.Instance.GameData.GetEvilArcherData(GameManagers.Instance.PlayerRecored.Level));
        }


        _role?.Register();
        gameObject.SetActive(true);

        _role.GetReady();
    }

    public void Reset()
    {
        gameObject.transform.position = _originPs;
        gameObject.SetActive(false);
        if (_charecter != null)
        {
            //_charecter.transform.position = new Vector3(_originPs.x, _originPs.y, _originPs.z);
            _role.Destroy();
            Destroy(_charecter);
            _charecter = null;
        }
    }

    public void RebuildEviel()
    {
        //gameObject.transform.position = _originPs;
        //if (_charecter != null)
        //{
        //    Destroy(_charecter);
        //}
    }


    private void Update()
    {
        if (!GameManagers.InGame || _role == null)
        {
            return;
        }

        _role.Update();
    }

    public void LateUpdate()
    {

        if (!GameManagers.InGame || _role == null)
        {
            return;
        }

        if (_role is ArcherHero)
        {
            ((ArcherHero)_role).LateUpdate();
        }

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

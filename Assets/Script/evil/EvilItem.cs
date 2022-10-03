using System.Collections;
using System.Collections.Generic;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class EvilItem : MonoBehaviour
{
    private Vector3 _originPs;
    private Role _role;

    private void Awake()
    {
        _originPs = gameObject.transform.position;
        //GameManagers.OnGameStart += () =>
        //{
        //    _role?.Register();
        //};
    }

    public void set(GameObject character)
    {
        _role = new EvilWarrior(character.GetComponent<Character>(), LevelManager.Instance.GameData.GetEvilWarriorData(1));
        _role?.Register();
    }

    private void Update()
    {
        if (!GameManagers.InGame || _role == null)
        {
            return;
        }

        _role.Update();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class HeroController
{
    Role _hero;
    public int HeroType;

    private bool _inGame;

    private GameObject _characterHost;

    private AudioSource ATKSource;
    private AudioSource ShootSource;

    public HeroController(AudioSource shootSource, AudioSource atkSource)
    {
        ATKSource = atkSource;
        ShootSource = shootSource;

        GameManagers.OnGameStart += () =>
        {
            _inGame = true;
            _hero?.Register();
        };
        GameManagers.OnGameEnd += onGameEnd;
    }

    public void RetTo(CharactorData data)
    {
        if (_hero != null)
        {
            _hero.Destroy();
        }

        _characterHost = data.Character;

        HeroType = data.Type;
        if (HeroType == HeroConstance.ARCHER)
        {
            _hero = new ArcherHero(data.Character.GetComponent<Character>(),
                LevelManager.Instance.GameData.GetArcherData(data.Level),
                () => { ShootSource.Play(); },
                () => { ATKSource.Play(); });
        }
        else
        {
            _hero = new WarriorHero(data.Character.GetComponent<Character>(),
                LevelManager.Instance.GameData.GetWarriorData(data.Level),
                () => { ShootSource.Play(); },
                () => { ATKSource.Play(); });
        }
    }

    public void Reset()
    {
        _hero.Destroy();
        _hero = null;
        GameObject.Destroy(_characterHost);
    }

    public void Update()
    {
        if (!_inGame || _hero == null)
        {
            return;
        }
        _hero.Update();
    }

    public void LateUpdate()
    {
        if (!_inGame || _hero == null)
        {
            return;
        }

        if (_hero is ArcherHero)
        {
            ((ArcherHero)_hero).LateUpdate();
        }

    }

    public void onGameEnd()
    {
        _inGame = false;
        _hero?.Idle();
    }
}

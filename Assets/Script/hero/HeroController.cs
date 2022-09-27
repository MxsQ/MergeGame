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

    public HeroController()
    {
        GameManagers.OnGameStart += () => { _inGame = true; };
        GameManagers.OnGameEnd += onGameEnd;
    }

    public void setTo(GameObject hero, int herotype)
    {
        if (_hero != null)
        {
            _hero.Destroy();
        }
        HeroType = herotype;
        if (herotype == HeroConstance.ARCHER)
        {
            _hero = new ArchorHero(hero.GetComponent<Character>());
        }
        else
        {
            _hero = new WorriorHero(hero.GetComponent<Character>());
        }
    }

    public void reset()
    {
        _hero.Destroy();
        _hero = null;
    }

    public void Update()
    {
        if (!_inGame)
        {
            return;
        }
        if (_hero == null)
        {
            return;
        }
        _hero.Update();
    }

    public void onGameEnd()
    {
        _inGame = false;
        _hero?.Idle();
    }
}

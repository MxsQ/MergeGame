using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class HeroController
{

    Role _hero;
    public int HeroType;

    public void setTo(GameObject hero, int herotype)
    {
        if (_hero != null)
        {
            _hero.Destroy();
        }
        HeroType = HeroType;
        if (herotype == HeroConstance.ARCHER)
        {
            _hero = new ArchorHero(hero.GetComponent<Character>());
        }
        else
        {
            _hero = new WorriorHero(hero.GetComponent<Character>());
        }
    }

    public void Update()
    {
        if (_hero == null)
        {
            return;
        }
        _hero.Update();
    }
}

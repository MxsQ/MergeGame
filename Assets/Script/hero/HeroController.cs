using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class HeroController
{
    GameObject _hero;
    Character character;
    public int HeroType;
    float _changeTime = 0;


    public void setTo(GameObject hero, int herotype)
    {
        _hero = hero;
        HeroType = herotype;
        character = hero.GetComponent<Character>();
        //if (herotype == HeroConstance.ARCHER)
        //{
        character.GetReady();
        //}
        //else {
        //character.Relax()
        //}
    }

    bool inChart = false;
    bool inRelase = false;

    public void Update()
    {
        if (character == null)
        {
            return;
        }

        _changeTime += Time.deltaTime;

        if (HeroType == HeroConstance.WORRIOR)
        {
            if (_changeTime >= 1)
            {
                _changeTime = 0;
                character.Slash();
            }
        }
        else
        {
            if (!inChart && !inRelase)
            {
                character.Animator.SetInteger("Charge", 1);
                inChart = true;
            }
            else if (inChart && _changeTime > 0.8)
            {
                character.Animator.SetInteger("Charge", 2);
                inChart = false;
                inRelase = true;
            }
            else if (inRelase && _changeTime > 1.4)
            {
                character.Animator.SetInteger("Charge", 0);
                inChart = false;
                inRelase = false;
                _changeTime = 0;
            }
        }

    }


    private void Shoot()
    {
        var originTransform = _hero.transform;
        var arrow = GameObject.Instantiate(GameManagers.Instance.Config.Arrow, originTransform);
        var sr = arrow.GetComponent<SpriteRenderer>();
        var rb = arrow.GetComponent<Rigidbody>();
        const float speed = 1300f;

        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.identity;
        arrow.transform.SetParent(null);
        sr.sprite = character.Bow.Single(j => j.name == "Arrow");
        rb.velocity = speed * new Vector3(character.transform.localScale.x, 0, 0);

        //var characterCollider = character.GetComponent<Collider>();
        //if (character != null)
        //{
        //    Physics.IgnoreCollision(arrow.GetComponent<Collider>(), characterCollider);
        //}
        //arrow.gameObject.layer = 31;
        //Physics.IgnoreLayerCollision(31, 31, true);
    }
}

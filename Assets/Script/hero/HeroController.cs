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
    }

    bool inChargeStart = false;
    bool inChargeMid = false;
    bool inChargeShoot = false;

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
            //if (!inChargeStart)
            //{
            //    inChargeStart = true;
            //    character.Animator.SetInteger("Charge", 2);
            //}
            //else if (inChargeStart && _changeTime > 1 && !inChargeMid)
            //{
            //    character.Animator.SetInteger("Charge", 2);
            //    inChargeMid = true;
            //}
            //else if (inChargeMid && _changeTime > 1.5 && !inChargeShoot)
            //{
            //    character.Animator.SetInteger("Charge", 3);
            //    inChargeShoot = true;
            //}
            //else if (inChargeShoot && _changeTime > 2)
            //{
            //    Shoot();
            //    _changeTime = 0;
            //    inChargeStart = false;
            //    inChargeMid = false;
            //    inChargeShoot = false;
            //}

            if (_changeTime > 2)
            {
                Shoot();
                _changeTime = 0;
            }

        }

    }

    public IEnumerator test()
    {
        character.Animator.SetInteger("Charge", 1); // 0 = ready, 1 = charging, 2 = release, 3 = cancel.

        yield return new WaitForSeconds(1);

        character.Animator.SetInteger("Charge", 2);

        yield return new WaitForSeconds(1);

        character.Animator.SetInteger("Charge", 0);
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

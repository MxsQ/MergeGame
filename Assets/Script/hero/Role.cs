﻿using Assets.HeroEditor.Common.CharacterScripts;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public abstract class Role
{
    protected Character _character;
    protected GameObject _parent;
    protected Vector3 _parentOriginPs;
    protected bool _isDestroy = false;

    protected float _changeTime = 0;

    protected int _curHP;
    protected RoleData _data;


    public Role(Character character, RoleData data)
    {
        _data = data;
        _character = character;
        _parent = _character.transform.parent.gameObject;
        _parentOriginPs = _parent.transform.position;
        character.GetReady();
        //Register();
    }

    public abstract void Update();

    public void Destroy()
    {
        _isDestroy = true;
        _character.transform.parent = null;
        GameObject.Destroy(_character);
        //unRegister();
    }

    public Vector3 Position()
    {
        return new Vector3(_character.gameObject.transform.position.x, _character.gameObject.transform.position.y, _character.gameObject.transform.position.z);
    }

    public void Idle()
    {
        _character.Animator.SetInteger("Charge", 0);
        _character.SetState(CharacterState.Ready);
    }

    public void BeHit(int damage)
    {
        _curHP -= damage;
    }

    protected abstract Role findTarget();
    public abstract void Register();
    public abstract void unRegister();
}

public class WarriorHero : Role
{
    private static int _attackRange = 90;

    private BoxCollider2D boxCollider;

    private int speed = 180;

    public WarriorHero(Character character, RoleData data) : base(character, data)
    {
        boxCollider = _parent.GetComponent<BoxCollider2D>();
    }

    public override void Update()
    {
        if (_isDestroy)
        {
            return;
        }

        _changeTime += Time.deltaTime;
        var enemy = findTarget();
        if (enemy == null)
        {
            return;
        }

        var targetPs = enemy.Position();

        if (boxCollider.bounds.Contains(targetPs))
        {
            _character.SetState(CharacterState.Ready);
            atack(enemy);
        }
        else
        {
            _character.SetState(CharacterState.Run);
            var direction = (targetPs - _parent.transform.position).normalized;
            _parent.transform.position += speed * direction * Time.deltaTime;
        }

        //if(Mathf.Abs(enemy.transform.position - _character.transform.position))
    }

    protected override Role findTarget()
    {
        return GameManagers.Instance.FindEnemy(_parent.gameObject);
    }

    public override void Register()
    {
        Debug.Log("human do");
        GameManagers.Instance.RegisterHero(this);
    }

    public override void unRegister()
    {
        GameManagers.Instance.UnRegisterHero(this);
    }

    private void atack(Role target)
    {
        if (_changeTime > 1)
        {
            _character.Slash();
            _changeTime = 0;
            DamageManagers.Instance.postDamage(target.Position(), _data.ATK);
        }
    }


}

public class ArcherHero : Role
{
    protected Sprite _arrow;
    private int _arrowIndex = 0;
    private int _armIndex = 0;
    private int weaponIndex = 3;

    private Transform _arm;
    private Transform _weapon;

    bool inChart = false;
    bool inRelase = false;
    bool inIdle = false;


    public ArcherHero(Character character, RoleData data) : base(character, data)
    {
        _arrow = character.Bow[_arrowIndex];
        _weapon = character.BodyRenderers[3].transform;
        CharacterBodySculptor sculptor = character.gameObject.GetComponent<CharacterBodySculptor>();
        _arm = sculptor.ArmL;
        //_arm.localEulerAngles = new Vector3(0, 0, 90);
        int i = 1;
        _arm.transform.rotation = Quaternion.EulerRotation(0, 0, 90);
    }


    public override void Update()
    {
        if (_isDestroy)
        {
            return;
        }

        _changeTime += Time.deltaTime;

        if (!inChart && !inRelase && !inIdle)
        {
            _character.Animator.SetInteger("Charge", 1);
            inChart = true;
        }
        else if (inChart && _changeTime > 0.5)
        {
            _character.Animator.SetInteger("Charge", 2);
            inChart = false;
            inRelase = true;
        }
        else if (inRelase && _changeTime > 0.8)
        {
            _character.Animator.SetInteger("Charge", 0);
            inRelase = false;
            inIdle = true;
        }
        else if (inIdle && _changeTime > 1)
        {
            inChart = false;
            inRelase = false;
            inIdle = false;
            _changeTime = 0;
            Shoot();
        }

        //RotateArm();
    }

    private void RotateArm()
    {
        //var target = GameManagers.Instance.FindEnemy(_character.gameObject);
        //if (target != null)
        //{
        //    var angleToTarget = Vector2.SignedAngle(Vector2.right, target.transform.position);
        //    var v = target.transform.position - _weapon.position;
        //    var fix = v.y / v.x;
        //    var angle = Mathf.Atan(fix) * Mathf.Rad2Deg;

        //    _arm.transform.localEulerAngles = new Vector3(0, 0, angle);

        //}

        var arm = _arm;
        var weapon = _weapon;
        var target = GameManagers.Instance.FindEnemy(_character.gameObject).Position();
        var angleMax = 180;
        var angleMin = -180;

        target = arm.transform.InverseTransformPoint(target);

        var angleToTarget = Vector2.SignedAngle(Vector2.right, target);
        var angleToArm = Vector2.SignedAngle(weapon.right, arm.transform.right) * Math.Sign(weapon.lossyScale.x);
        var fix = weapon.InverseTransformPoint(arm.transform.position).y / target.magnitude;

        //AngleToTarget = angleToTarget;
        //AngleToArm = angleToArm;

        if (fix < -1) fix = -1;
        else if (fix > 1) fix = 1;

        var angleFix = Mathf.Asin(fix) * Mathf.Rad2Deg;
        var angle = angleToTarget + angleFix + arm.transform.localEulerAngles.z;

        angle = NormalizeAngle(angle);

        if (angle > angleMax)
        {
            angle = angleMax;
        }
        else if (angle < angleMin)
        {
            angle = angleMin;
        }

        if (float.IsNaN(angle))
        {
            Debug.LogWarning(angle);
        }

        arm.transform.localEulerAngles = new Vector3(0, 0, angle + angleToArm);
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;

        return angle;
    }

    private void Shoot()
    {
        Role target = findTarget();
        if (target == null)
        {
            return;
        }

        var originPs = _character.transform.position;
        //_arrow.gameObject.SetActive(true);
        var arrow = GameObject.Instantiate(_arrow);

        var arrowObject = new GameObject();
        SpriteRenderer spr = arrowObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spr.sprite = arrow;
        spr.sortingOrder = 10;
        arrowObject.transform.position = new Vector3(originPs.x, originPs.y, originPs.z);
        arrowObject.transform.localScale = new Vector3(25, 25, 0);



        ArrowManager.Instance.Shoot(arrowObject, target, _data.ATK);
    }

    protected override Role findTarget()
    {
        return GameManagers.Instance.FindEnemy(_character.gameObject);
    }

    public override void Register()
    {
        GameManagers.Instance.RegisterHero(this);
    }

    public override void unRegister()
    {
        GameManagers.Instance.UnRegisterHero(this);
    }
}

public class EvilWarrior : WarriorHero
{
    public EvilWarrior(Character character, RoleData data) : base(character, data)
    {
    }

    protected override Role findTarget()
    {
        return GameManagers.Instance.FindHero(_character.gameObject);
    }

    public override void Register()
    {
        Debug.Log("evil do");
        GameManagers.Instance.RegisterEnemy(this);
    }

    public override void unRegister()
    {
        GameManagers.Instance.UnRegisterEnemy(this);
    }
}

public class EvilArcher : ArcherHero
{
    public EvilArcher(Character character, RoleData data) : base(character, data)
    {

    }

    protected override Role findTarget()
    {
        return GameManagers.Instance.FindHero(_character.gameObject);
    }

    public override void Register()
    {
        GameManagers.Instance.RegisterEnemy(this);
    }

    public override void unRegister()
    {
        GameManagers.Instance.UnRegisterEnemy(this);
    }
}
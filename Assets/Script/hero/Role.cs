using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.CharacterScripts.Firearms;
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

    protected float _atkSpand;
    protected bool _die = false;


    public Role(Character character, RoleData data)
    {
        _data = data;
        _character = character;
        _parent = _character.transform.parent.gameObject;
        _parentOriginPs = _parent.transform.position;
        character.GetReady();
        //Register();
        _atkSpand = 1 + (new System.Random().Next(0, 10) - 5) / 100;
        _curHP = data.HP;
    }

    public abstract void Update();

    public void GetReady()
    {
        _character.GetReady();
    }

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

    public virtual void BeHit(int damage)
    {
        _curHP -= damage;
        if (_curHP <= 0)
        {
            _die = true;
            OnDie();
        }

    }

    public int GetCurHP() { return _curHP; }

    protected abstract Role findTarget();
    public abstract void Register();
    public abstract void unRegister();
    public abstract void OnDie();
}

public class WarriorHero : Role
{
    private static int _attackRange = 90;

    private BoxCollider2D boxCollider;

    private float speed;

    public WarriorHero(Character character, RoleData data) : base(character, data)
    {
        boxCollider = _parent.GetComponent<BoxCollider2D>();
        speed = GameManagers.Instance.Config.MoveSpeed;
    }

    public override void Update()
    {
        if (_isDestroy)
        {
            return;
        }

        _changeTime += Time.deltaTime;

        if (_die)
        {
            _character.SetState(CharacterState.DeathF);
            return;
        }

        var enemy = findTarget();
        if (enemy == null)
        {
            return;
        }

        var targetPs = enemy.Position();

        //Debug.Log("my position=" + _character.gameObject.transform.position + "   target position=" + targetPs);
        if (boxCollider.bounds.Contains(targetPs))
        {
            //Debug.Log("target: " + targetPs);
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
        GameManagers.Instance.RegisterHero(this);
    }

    public override void unRegister()
    {
        GameManagers.Instance.UnRegisterHero(this);
    }

    private void atack(Role target)
    {
        if (_changeTime > _atkSpand)
        {
            _character.Slash();
            AudioManager.Instance.PlayAttack();
            _changeTime = 0;
            DamageManagers.Instance.postDamage(target.Position(), _data.ATK, () => { target.BeHit(_data.ATK); }, 0.3f);
        }
    }

    public override void OnDie()
    {
        GameManagers.Instance.OnHeroDeath(this);
        //_character.SetState(CharacterState.DeathF);
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
    private Vector3 _nextShootTarget;
    private bool _hasShootTarget = false;

    bool inChart = false;
    bool inRelase = false;
    bool inIdle = false;


    public ArcherHero(Character character, RoleData data) : base(character, data)
    {
        _arrow = character.Bow[_arrowIndex];
        _weapon = character.BodyRenderers[3].transform;
        CharacterBodySculptor sculptor = character.gameObject.GetComponent<CharacterBodySculptor>();
        _arm = sculptor.ArmL;

    }



    public override void Update()
    {
        if (_isDestroy)
        {
            return;
        }

        _changeTime += Time.deltaTime;

        if (_die)
        {
            _character.SetState(CharacterState.DeathF);
            return;
        }

        Role target = findTarget();
        if (target == null)
        {
            _character.Relax();
            _hasShootTarget = false;
            return;
        }

        _hasShootTarget = true;
        _nextShootTarget = target.Position();

        if (!inChart && !inRelase && !inIdle)
        {
            _character.Animator.SetInteger("Charge", 1);
            inChart = true;
        }
        else if (inChart && _changeTime > _atkSpand * 0.5)
        {
            _character.Animator.SetInteger("Charge", 2);
            inChart = false;
            inRelase = true;
        }
        else if (inRelase && _changeTime > _atkSpand * 0.8)
        {
            _character.Animator.SetInteger("Charge", 0);
            inRelase = false;
            inIdle = true;
        }
        else if (inIdle && _changeTime > _atkSpand)
        {
            inChart = false;
            inRelase = false;
            inIdle = false;
            _changeTime = 0;
            Shoot(target);
        }

        //RotateArm();
    }

    public void LateUpdate()
    {
        var state = _character.GetState();
        if (state == CharacterState.DeathB || state == CharacterState.DeathF)
        {
            return;
        }

        Debug.Log("adjust position");
        if (_hasShootTarget)
        {
            RotateArm(_arm, _weapon, _nextShootTarget, -40, 40);
        }
        else
        {
            RotateArm(_arm, _weapon, _arm.position + 1000 * Vector3.right, -40, 40);
        }
    }




    private float AngleToTarget;
    private float AngleToArm;

    public void RotateArm(Transform arm, Transform weapon, Vector2 target, float angleMin, float angleMax) // TODO: Very hard to understand logic.
    {


        target = arm.transform.InverseTransformPoint(target);
        //Debug.Log("new position" + target);

        var angleToTarget = Vector2.SignedAngle(Vector2.right, target);
        var angleToArm = Vector2.SignedAngle(weapon.right, arm.transform.right) * Math.Sign(weapon.lossyScale.x);
        var fix = weapon.InverseTransformPoint(arm.transform.position).y / target.magnitude;

        AngleToTarget = angleToTarget;
        AngleToArm = angleToArm;

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

    private void Shoot(Role target)
    {
        if (target == null)
        {
            return;
        }
        AudioManager.Instance.PlayShoot();

        var originPs = _character.transform.position;
        //_arrow.gameObject.SetActive(true);
        var arrow = GameObject.Instantiate(_arrow);

        var arrowObject = new GameObject();
        SpriteRenderer spr = arrowObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spr.sprite = arrow;
        spr.sortingOrder = 10;
        var rePosition = _arm.transform.position;
        arrowObject.transform.position = new Vector3(rePosition.x, rePosition.y, rePosition.z);
        var scale = GameManagers.Instance.Config.ArrowSizeScale;
        arrowObject.transform.localScale = new Vector3(scale, scale, 0);



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

    public override void OnDie()
    {
        GameManagers.Instance.OnHeroDeath(this);
        //_character.SetState(CharacterState.DeathF);
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
        GameManagers.Instance.RegisterEnemy(this);
    }

    public override void unRegister()
    {
        GameManagers.Instance.UnRegisterEnemy(this);
    }

    public override void OnDie()
    {
        AudioManager.Instance.PlayEvilDie();
        GameManagers.Instance.OnEvilDeath(this);
    }


    public override void BeHit(int damage)
    {
        base.BeHit(damage);
        EvilManager.Instance.OnEvilBeHit();
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

    public override void OnDie()
    {
        AudioManager.Instance.PlayEvilDie();
        GameManagers.Instance.OnEvilDeath(this);
    }

    public override void BeHit(int damage)
    {
        base.BeHit(damage);
        EvilManager.Instance.OnEvilBeHit();
    }
}
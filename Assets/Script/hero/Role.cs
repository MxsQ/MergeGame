using Assets.HeroEditor.Common.CharacterScripts;
using System.Collections;
using System.Linq;
using UnityEngine;


public abstract class Role
{
    protected Character _character;
    protected GameObject _parent;
    protected Vector3 _parentOriginPs;

    protected float _changeTime = 0;

    public Role(Character character)
    {
        _character = character;
        _parent = _character.transform.parent.gameObject;
        _parentOriginPs = _parent.transform.position;
        character.GetReady();
    }

    public abstract void Update();

    public void Destroy()
    {
        _character.transform.parent = null;
        GameObject.Destroy(_character);
    }

}

public class WorriorHero : Role
{
    private static int _attackRange = 90;

    private BoxCollider2D boxCollider;

    private int speed = 180;

    public WorriorHero(Character character) : base(character)
    {
        boxCollider = _parent.GetComponent<BoxCollider2D>();
    }

    public override void Update()
    {
        _changeTime += Time.deltaTime;
        var enemy = GameManagers.Instance.FindEnemy(_parent.gameObject);
        var targetPs = enemy.transform.position;
        if (enemy == null)
        {
            _character.SetState(CharacterState.Run);
            return;
        }

        if (boxCollider.bounds.Contains(targetPs))
        {
            _character.SetState(CharacterState.Ready);
            atack();
        }
        else
        {
            var direction = (targetPs - _parent.transform.position).normalized;
            _parent.transform.position += speed * direction * Time.deltaTime;
        }

        //if(Mathf.Abs(enemy.transform.position - _character.transform.position))
    }

    private void atack()
    {
        if (_changeTime > 1)
        {
            _character.Slash();
            _changeTime = 0;
        }
    }

    private void moveToEnemy()
    {

    }
}

public class ArchorHero : Role
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


    public ArchorHero(Character character) : base(character)
    {
        _arrow = character.Bow[_arrowIndex];
        _weapon = character.BodyRenderers[3].transform;
        CharacterBodySculptor sculptor = character.gameObject.GetComponent<CharacterBodySculptor>();
        _arm = sculptor.ArmL;
        //craeteAShoo();
    }


    public override void Update()
    {
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
    }

    private void RotateArm()
    {
        var targePs = GameManagers.Instance.FindEnemy(_character.gameObject).transform.position;
        //var angle = angle(targePs, _character.transform.position);
    }

    private void craeteAShoo()
    {
        var originPs = _character.transform.position;
        //_arrow.gameObject.SetActive(true);
        var arrow = GameObject.Instantiate(_arrow);

        var arrowObject = new GameObject();
        SpriteRenderer spr = arrowObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spr.sprite = arrow;
        arrowObject.transform.position = new Vector3(originPs.x, originPs.y, originPs.z);
        arrowObject.transform.localScale = new Vector3(25, 25, 0);

        arrowObject.layer = 10;
    }

    private void Shoot()
    {
        var originPs = _character.transform.position;
        //_arrow.gameObject.SetActive(true);
        var arrow = GameObject.Instantiate(_arrow);

        var arrowObject = new GameObject();
        SpriteRenderer spr = arrowObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spr.sprite = arrow;
        arrowObject.transform.position = new Vector3(originPs.x, originPs.y, originPs.z);
        arrowObject.transform.localScale = new Vector3(25, 25, 0);
        arrowObject.layer = 10;


        GameObject target = GameManagers.Instance.FindEnemy(_character.gameObject);
        ArrowManager.Instance.Shoot(arrowObject, target.transform.position);
    }
}

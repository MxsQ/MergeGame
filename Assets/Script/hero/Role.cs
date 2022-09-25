using Assets.HeroEditor.Common.CharacterScripts;
using System.Collections;
using UnityEngine;


public abstract class Role
{
    protected Character _character;

    protected float _changeTime = 0;


    public Role(Character character)
    {
        _character = character;
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
    public WorriorHero(Character character) : base(character)
    {

    }

    public override void Update()
    {
        _changeTime += Time.deltaTime;
        if (_changeTime > 1)
        {
            _character.Slash();
            _changeTime = 0;
        }

    }
}

public class ArchorHero : Role
{
    protected GameObject _arrow;

    bool inChart = false;
    bool inRelase = false;
    bool inIdle = false;

    public ArchorHero(Character character) : base(character)
    {
        //Debug.Log(character.transform.parent.gameObject.name);


        object tmp = GameObject.Find($"{character.name}/Animation/Body/Upper/ArmL/ForearmL[2]/HandL/Bow/FireTransform/Arrow");

        //_arrow = (tmp as GameObject).GetComponent<SpriteRenderer>();
        _arrow = tmp as GameObject;
        _arrow.SetActive(true);
        //int a = 1;
        //GameManagers.Instance.StartCoroutine(findObject());
    }

    //public IEnumerator findObject()
    //{
    //    yield return new WaitForSeconds(1);

    //    object tmp = GameObject.Find("archor0(Clone)/Animation/Body/Upper/ArmL/ForearmL[2]/HandL/Bow/FireTransform/Arrow");
    //    //object tmp = _character.FindW("Arrow");
    //    //_arrow = (tmp as GameObject).GetComponent<SpriteRenderer>();
    //    _arrow = tmp as GameObject;
    //    _arrow.SetActive(true);
    //}

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

    private void Shoot()
    {
        //var originPs = _character.transform.position;
        //_arrow.gameObject.SetActive(true);
        //var arrow = GameObject.Instantiate(_arrow.sprite);

        //var arrowObject = new GameObject();
        //SpriteRenderer spr = arrowObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        //spr.sprite = arrow;
        //arrowObject.transform.position = new Vector3(originPs.x, originPs.y, originPs.z);

        //GameObject target = GameManagers.Instance.FindEnemy(_character);
        //ArrowManager.Instance.Shoot(arrowObject, target.transform.position);
    }
}

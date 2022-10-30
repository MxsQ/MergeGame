using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance
    {
        get { return _instance; }
    }

    private static ArrowManager _instance;

    public List<TheArrow> _arrows = new List<TheArrow>();

    private void Awake()
    {
        _instance = this;
    }

    private void FixedUpdate()
    {
        var changTime = Time.deltaTime;
        List<TheArrow> hittedArrow = new List<TheArrow>();


        foreach (TheArrow a in _arrows)
        {
            a.KeepMove(changTime);
            if (a.IsHit())
            {
                hittedArrow.Add(a);
                if (!a.fromEvil)
                {
                    DamageManagers.Instance.postDamage(a._targetPs, a._damage, null);
                }
            }
        }

        foreach (TheArrow a in hittedArrow)
        {
            a.Dohit();
            _arrows.Remove(a);
            GameObject.Destroy(a.Arrow);
        }
    }

    public void Shoot(GameObject arrow, Role target, int damage, bool fromEvil)
    {
        _arrows.Add(new TheArrow(arrow, target, damage, fromEvil));
    }
}

public class TheArrow
{
    public GameObject Arrow;
    public Vector3 _targetPs;
    private Role _target;
    private Vector3 _direction;
    public int _damage;
    public bool fromEvil;

    private float _maxFlyTime;
    private float _flyTime = 0;
    private Vector3 _speed;

    public TheArrow(GameObject arrow, Role target, int damage, bool evil)
    {
        Arrow = arrow;
        _damage = damage;
        _target = target;
        fromEvil = evil;
        _targetPs = _target.Position();

        var disV = _targetPs - arrow.transform.position;
        var x = (_targetPs.x - arrow.transform.position.x);
        var y = (_targetPs.y - arrow.transform.position.y);
        var dis = Mathf.Sqrt(x * x + y * y);
        var speedFactory = GameManagers.Instance.Config.ArrowSpeedFactory;
        var speed = GameManagers.Instance.Config.ArrowSpeed * speedFactory;
        _direction = disV.normalized;

        _maxFlyTime = dis / speed / speedFactory;
        Debug.Log("distance=" + dis);
        _speed = disV / _maxFlyTime;


        Vector3 v = _targetPs - arrow.transform.position;
        var fix = v.y / v.x;
        var angle = Mathf.Atan(fix) * Mathf.Rad2Deg;

        if (!fromEvil)
        {
            arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
        else
        {
            arrow.transform.localEulerAngles = new Vector3(0, 0, angle + 180);
        }

    }

    public void KeepMove(float delta)
    {
        _flyTime += delta;
        Arrow.transform.position += delta * _speed;
    }

    public bool IsHit()
    {
        //var ps = Arrow.transform.position;
        //return Mathf.Abs(ps.x - _targetPs.x) < 10 && Mathf.Abs(ps.y - _targetPs.y) < 10;
        return _flyTime > _maxFlyTime;
    }

    public void Dohit()
    {
        _target.BeHit(_damage);
    }
}

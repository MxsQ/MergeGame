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
                DamageManagers.Instance.postDamage(a._targetPs, a._damage);
            }
        }

        foreach (TheArrow a in hittedArrow)
        {
            _arrows.Remove(a);
            GameObject.Destroy(a.Arrow);
        }
    }

    public void Shoot(GameObject arrow, Role target, int damage)
    {
        _arrows.Add(new TheArrow(arrow, target, damage));
    }
}

public class TheArrow
{
    public GameObject Arrow;
    public Vector3 _targetPs;
    private Role _target;
    private Vector3 _speed;
    public int _damage;

    public TheArrow(GameObject arrow, Role target, int damage)
    {
        Arrow = arrow;
        _damage = damage;
        _target = target;
        _targetPs = _target.Position();
        _speed = (_targetPs - arrow.transform.position);


        Vector3 v = _targetPs - arrow.transform.position;
        var fix = v.y / v.x;
        var angle = Mathf.Atan(fix) * Mathf.Rad2Deg;

        arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void KeepMove(float delta)
    {
        Arrow.transform.position += delta * 2f * _speed;
    }

    public bool IsHit()
    {
        var ps = Arrow.transform.position;
        return Mathf.Abs(ps.x - _targetPs.x) < 10 && Mathf.Abs(ps.y - _targetPs.y) < 10;
    }
}

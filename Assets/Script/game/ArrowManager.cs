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
            }
        }

        foreach (TheArrow a in hittedArrow)
        {
            _arrows.Remove(a);
            GameObject.Destroy(a.Arrow);
        }
    }

    public void Shoot(GameObject arrow, Vector3 targetPs)
    {
        _arrows.Add(new TheArrow(arrow, targetPs));
    }
}

public class TheArrow
{
    public GameObject Arrow;
    private Vector3 _targetPs;
    private Vector3 _speed;

    public TheArrow(GameObject arrow, Vector3 targetPs)
    {
        Arrow = arrow;
        _targetPs = targetPs;
        _speed = (targetPs - arrow.transform.position);


        Vector3 v = targetPs - arrow.transform.position;
        var fix = v.y / v.x;
        var angle = Mathf.Atan(fix) * Mathf.Rad2Deg;

        arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void KeepMove(float delta)
    {
        Arrow.transform.position += delta * _speed;
    }

    public bool IsHit()
    {
        var ps = Arrow.transform.position;
        return Mathf.Abs(ps.x - _targetPs.x) < 10 && Mathf.Abs(ps.y - _targetPs.y) < 10;
    }
}

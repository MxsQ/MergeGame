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

    private void Update()
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
        Vector3 rotate = new Vector3(0, 0, angle(targetPs, arrow.transform.position) - 90);
        arrow.transform.rotation = Quaternion.Euler(rotate);
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

    private float angle(Vector3 from, Vector3 to)
    {
        var v = Vector3.Dot(from.normalized, to.normalized);
        float angle = Mathf.Acos(v);
        angle *= Mathf.Rad2Deg;
        return angle;
    }
}

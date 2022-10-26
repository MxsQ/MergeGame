using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageManagers : MonoBehaviour
{
    [SerializeField] GameObject HitHost;

    private List<TextHolder> _textHoders = new List<TextHolder>();

    private static DamageManagers _instance;
    public static DamageManagers Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
        GameManagers.OnGameStart += () => { _textHoders.Clear(); };

    }

    public void postDamage(Vector3 target, int damage, Action onComplete, float delay = 0)
    {
        if (target != null)
        {
            StartCoroutine(DoHit(delay, () =>
            {
                var o = GameObject.Instantiate(GameManagers.Instance.Config.HitText);
                o.transform.SetParent(HitHost.transform);
                o.transform.position = Camera.main.WorldToScreenPoint(target + new Vector3(90, 180, 0));
                o.GetComponentInChildren<Text>().text = damage.ToString();
                _textHoders.Add(new TextHolder(o));

                onComplete?.Invoke();
            }));

        }
    }

    private IEnumerator DoHit(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action.Invoke();
    }

    private void Update()
    {
        List<TextHolder> tmp = new List<TextHolder>();
        foreach (TextHolder t in _textHoders)
        {
            if (t.HaveToRemove())
            {
                tmp.Add(t);
            }
        }

        foreach (TextHolder t in tmp)
        {
            _textHoders.Remove(t);
            GameObject.Destroy(t._target);
        }

        foreach (TextHolder t in _textHoders)
        {
            t.Update();
        }
    }
}


public class TextHolder
{

    public GameObject _target;
    private Vector3 _speed = new Vector3(0, 180, 0);
    private float _changeTime;

    public TextHolder(GameObject o)
    {
        _target = o;
    }

    public void Update()
    {
        _changeTime += Time.deltaTime;
        _target.transform.position += _speed * Time.deltaTime;
    }

    public bool HaveToRemove()
    {
        return _changeTime > 0.8;
    }

}

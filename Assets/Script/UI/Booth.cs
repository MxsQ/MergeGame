using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booth : MonoBehaviour
{
    [SerializeField] GameObject NormalBG;
    [SerializeField] GameObject SelectBG;
    [SerializeField] GameObject LockBG;
    [SerializeField] GameObject ShowPS;

    public void ShowRole(GameObject role)
    {
        role.transform.parent = ShowPS.transform;
        var ps = ShowPS.transform.position;
        role.transform.position = new Vector3(ps.x, ps.y, ps.z);
        role.transform.localScale = new Vector3(46, 46, 1);

        SelectBG.SetActive(false);
        LockBG.SetActive(false);
        NormalBG.SetActive(true);
    }

}

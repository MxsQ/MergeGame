using System.Collections;
using System.Collections.Generic;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class Test : MonoBehaviour
{
    Character Character;

    bool down = false;
    bool up = false;

    public void OnValidate()
    {
        Character = FindObjectOfType<Character>();
        Character.Animator.SetBool("Ready", true);
    }

    public void OnDown()
    {
        Debug.Log("call down");
        //down = true;
        Character.Animator.SetInteger("Charge", 1);
    }

    public void OnUP()
    {
        //down = false;
        //up = true;
        Debug.Log("call up");
        //Character.Animator.SetInteger("Charge", 2);
        Character.Animator.SetInteger("Charge", 2);
    }

    public void OnRelease()
    {
        Debug.Log("call release");
        Character.Animator.SetInteger("Charge", 0);
    }

    float changTime = 0;

    //public void Update()
    //{
    //    if (down)
    //    {

    //        //Character.Animator.SetTrigger("SimpleBowShot");

    //        Character.Animator.SetInteger("Charge", 1);
    //    }

    //    if (up)
    //    {
    //        Character.Animator.SetInteger("Charge", 2);
    //    }

    //    //changTime += Time.deltaTime;
    //    //if (changTime > 0)
    //    //{
    //    //    Character.Animator.SetInteger("Charge", 1);
    //    //}
    //    //else if (changTime > 1)
    //    //{
    //    //    Character.Animator.SetInteger("Charge", 2);
    //    //}
    //    //else if (changTime > 1.8)
    //    //{
    //    //    Character.Animator.SetInteger("Charge", 0);
    //    //    changTime = 0;
    //    //}
    //}
}

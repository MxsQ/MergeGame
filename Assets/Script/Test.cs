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
    }

    public void OnDown()
    {
        Debug.Log("call down");
        down = true;

    }

    public void OnUP()
    {
        down = false;
        up = true;
        Debug.Log("call up");
        //Character.Animator.SetInteger("Charge", 2);
    }

    public void Update()
    {
        if (down)
        {

            Character.Animator.SetInteger("Charge", 1);
        }

        if (up)
        {

            Character.Animator.SetInteger("Charge", 2);
        }
    }
}

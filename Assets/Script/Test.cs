using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.CharacterScripts.Firearms;
using UnityEngine;

public class Test : MonoBehaviour
{
    Character Character;

    bool down = false;
    bool up = false;

    Transform _weapon;
    Transform _arm;

    public Transform TestArm;

    public void Awake()
    {
        Character = FindObjectOfType<Character>();
        //Character.Animator.SetBool("Ready", true);

        Character.GetReady();

        _weapon = Character.BowRenderers[3].transform;
        CharacterBodySculptor sculptor = Character.gameObject.GetComponent<CharacterBodySculptor>();
        _arm = sculptor.ArmR[0];

        //_arm.transform.localEulerAngles = new Vector3(0, 0, 100);
    }

    private float AngleToTarget;
    private float AngleToArm;

    public void RotateArm(Transform arm, Transform weapon, Vector2 target, float angleMin, float angleMax) // TODO: Very hard to understand logic.
    {


        target = arm.transform.InverseTransformPoint(target);
        //Debug.Log("new position" + target);

        var angleToTarget = Vector2.SignedAngle(Vector2.right, target);
        var angleToArm = Vector2.SignedAngle(weapon.right, arm.transform.right) * Math.Sign(weapon.lossyScale.x);
        var fix = weapon.InverseTransformPoint(arm.transform.position).y / target.magnitude;

        AngleToTarget = angleToTarget;
        AngleToArm = angleToArm;

        if (fix < -1) fix = -1;
        else if (fix > 1) fix = 1;

        var angleFix = Mathf.Asin(fix) * Mathf.Rad2Deg;
        var angle = angleToTarget + angleFix + arm.transform.localEulerAngles.z;

        angle = NormalizeAngle(angle);

        if (angle > angleMax)
        {
            angle = angleMax;
        }
        else if (angle < angleMin)
        {
            angle = angleMin;
        }

        if (float.IsNaN(angle))
        {
            Debug.LogWarning(angle);
        }

        var adjust = -angle - angleToArm;
        adjust -= -2 * angle;
        Debug.Log("angle=" + angle + "  angleToArm=" + angleToArm + "  showAdjust=" + adjust);

        arm.transform.localEulerAngles = new Vector3(0, 0, adjust);
    }

    private void LateUpdate()
    {
        RotateArm(TestArm, _weapon, Camera.main.ScreenToWorldPoint(Input.mousePosition), -40, 40);

    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;

        return angle;
    }

    //public void load()

    public void load()
    {
        //yield return new WaitForSeconds(1);

        var jsonString = System.IO.File.ReadAllText(Application.dataPath + "/Resources/herojson/hero.json");
        Debug.Log(jsonString);
        //Character.Initialize();
        Character.FromJson(jsonString);
    }

    public void OnDown()
    {
        Debug.Log("call down");
        ////down = true;
        Character.Animator.SetInteger("Charge", 1);


        //TestArm.transform.localEulerAngles = new Vector3(0, 0, 100);
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

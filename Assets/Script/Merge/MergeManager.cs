using System.Collections;
using System.Collections.Generic;
using Assets.HeroEditor.Common.CharacterScripts;
using UnityEngine;

public class MergeManager : MonoBehaviour
{

    [SerializeField] MergeItem[] mergeItems;
    //[SerializeField] GameObject warriorPrafab;
    [SerializeField] CharacterScriptableObject WorriorCharaterConfig;
    [SerializeField] CharacterScriptableObject ArchorCharaterConfig;
    [SerializeField] Character archor;

    private TOUCH_STATE _curTouchState = TOUCH_STATE.IDLE;
    private MergeItem _curItem;


    private void Awake()
    {
        Debug.Log("awake");
    }

    void Update()
    {
        var touchPs = GetScreenPosition();
        if (_curTouchState == TOUCH_STATE.START)
        {
            _curItem = findBy(touchPs);
            //Log.D("touch start: " + touchPs);
        }
        else if (_curTouchState == TOUCH_STATE.MOUVE)
        {
            //Log.D("touch move: " + touchPs);
            _curItem?.MoveTo(touchPs);
        }
        else if (_curTouchState == TOUCH_STATE.END)
        {
            if (_curItem == null)
            {
                return;
            }

            var mergeTarget = findBy(touchPs);
            if (_curItem == mergeTarget || !_curItem.canbeMerge(mergeTarget))
            {
                _curItem?.MoveToOriginal();
            }
            else
            {
                int newLevel = mergeTarget.CharacterLevel + 1;
                GameObject newCharacter = getWorriorCharacterBy(newLevel);
                mergeTarget.mergetCharactor(newCharacter);
                _curItem.Reset();
            }

            _curItem = null;
            //Log.D("touch end: " + touchPs);
        }
    }

    MergeItem findBy(Vector2 screenPs)
    {
        MergeItem target = null;
        foreach (MergeItem t in mergeItems)
        {
            if (t.InZone(screenPs))
            {
                target = t;
                break;
            }
        }

        return target;
    }

    GameObject getWorriorCharacterBy(int index)
    {
        GameObject character = (GameObject)Instantiate(WorriorCharaterConfig.characters[index]);
        return character;
    }

    GameObject getArchorCharacterBy(int index)
    {
        GameObject character = (GameObject)Instantiate(ArchorCharaterConfig.characters[index]);
        return character;
    }

    Vector2 GetScreenPosition()
    {
        /*
         uses monse
         */
        if (Input.GetMouseButtonDown(0))
        {
            _curTouchState = TOUCH_STATE.START;
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _curTouchState = TOUCH_STATE.END;
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else if (Input.GetMouseButton(0))
        {
            _curTouchState = TOUCH_STATE.MOUVE;
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }


        /*
         uses finger
         */
        if (Input.touches.Length > 0)
        {
            var touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                _curTouchState = TOUCH_STATE.START;
                return touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                _curTouchState = TOUCH_STATE.MOUVE;
                return touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _curTouchState = TOUCH_STATE.END;
                return touch.position;
            }


        }


        return new Vector2(0, 0);
    }

    public void OnAddWarriorClick()
    {
        //Resources.Load()
        MergeItem cur = selectEmptyItem();
        cur?.setCharector(getWorriorCharacterBy(0), HeroConstance.WORRIOR);

    }

    public void OnAddArcherClick()
    {
        MergeItem cur = selectEmptyItem();
        cur?.setCharector(getArchorCharacterBy(0), HeroConstance.ARCHER);
    }

    private MergeItem selectEmptyItem()
    {
        MergeItem cur = null;
        foreach (MergeItem t in mergeItems)
        {
            if (!t.HasCharesctor)
            {
                cur = t;
            }
        }
        return cur;
    }

    enum TOUCH_STATE
    {
        IDLE,
        START,
        MOUVE,
        END
    }
}

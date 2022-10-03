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

    private TOUCH_STATE _curTouchState = TOUCH_STATE.IDLE;
    private MergeItem _curItem;

    private bool _inGame;

    private static MergeManager _instance;
    public static MergeManager Instance
    {
        get { return _instance; }
    }


    private void Awake()
    {
        _instance = this;
        Debug.Log("awake");
        GameManagers.OnGameStart += () => { _inGame = true; };
        GameManagers.OnGameEnd += OnGameEnd;
    }

    void Update()
    {
        if (_inGame)
        {
            return;
        }
        var touchPs = GetScreenPosition();
        if (_curTouchState == TOUCH_STATE.START)
        {
            _curItem = findBy(touchPs);
            //Log.D("touch start: " + touchPs);
        }
        else if (_curTouchState == TOUCH_STATE.MOUVE)
        {
            //Log.D("touch move: " + touchPs);
            if (_curItem == null || !_curItem.HasCharesctor)
            {
                return;
            }
            _curItem.MoveTo(touchPs);
        }
        else if (_curTouchState == TOUCH_STATE.END)
        {
            if (_curItem == null || !_curItem.HasCharesctor)
            {
                return;
            }

            var mergeTarget = findBy(touchPs);
            if (_curItem != mergeTarget && mergeTarget != null && _curItem.canbeMerge(mergeTarget))
            {
                // merge
                int newLevel = mergeTarget.CurCharacterData.Level + 1;
                GameObject newCharacter = getCharacterBy(mergeTarget.CurCharacterData.Type, newLevel);
                mergeTarget.mergetCharactor(newCharacter);
                _curItem.Reset();
            }
            else if (_curItem != mergeTarget && mergeTarget != null && !mergeTarget.HasCharesctor)
            {
                // move to new position
                _curItem.MoveToOriginal();
                mergeTarget.MoveFrom(_curItem);
            }
            else if (_curItem != mergeTarget && mergeTarget != null && mergeTarget.CanbeExchangePosition(_curItem))
            {
                // exchange each position
                mergeTarget.ExchangeTo(_curItem);
                _curItem.MoveToOriginal();
                mergeTarget.MoveToOriginal();
            }
            else
            {
                // move to original position
                _curItem.MoveToOriginal();
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

    GameObject getCharacterBy(int type, int index)
    {
        if (type == HeroConstance.ARCHER)
        {
            return getArchorCharacterBy(index);
        }

        return getWorriorCharacterBy(index);
    }

    GameObject getWorriorCharacterBy(int index)
    {
        //GameObject character = (GameObject)Instantiate(WorriorCharaterConfig.characters[index]);
        //return character;
        return GameManagers.Instance.GetWarriorCharacter(index);
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
        cur?.setCharector(new CharactorData(getWorriorCharacterBy(0), HeroConstance.WORRIOR));

    }

    public void OnAddArcherClick()
    {
        MergeItem cur = selectEmptyItem();
        cur?.setCharector(new CharactorData(getArchorCharacterBy(0), HeroConstance.ARCHER));
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

    private void OnGameEnd()
    {
        _inGame = false;
        foreach (MergeItem i in mergeItems)
        {
            if (i.HasCharesctor)
            {
                i.MoveToOriginal();
            }
        }
    }

    enum TOUCH_STATE
    {
        IDLE,
        START,
        MOUVE,
        END
    }
}

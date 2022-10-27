using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    [SerializeField] GameObject Container;
    [SerializeField] GameObject UI;
    [SerializeField] Booth[] Booths;
    [SerializeField] Image[] Indicator;

    [SerializeField] Sprite SelectImg;
    [SerializeField] Sprite UnSelectImg;

    [SerializeField] Text SkinName;

    private string[] _skinName = new string[] { "Padadin", "Elf Archer", "Assassin", "Berserker", "Undead Archer", "Cavalier" };

    private RoleSkin[] _skins = new RoleSkin[]{
        RoleSkin.WARRIOR_DEFAUL,
        RoleSkin.ARCHER_DEFAUL,
        RoleSkin.WARRIOR_1,
        RoleSkin.WARRIOR_2,
        RoleSkin.ARCHER_1,
        RoleSkin.WARRIOR_3,
    };

    private RoleSkin _curSkin = RoleSkin.WARRIOR_DEFAUL;
    private int skinIndex = 0;

    public void Show()
    {
        Container.SetActive(true);
        UI.SetActive(true);
        RefreshBooth();
    }

    public void RefreshBooth()
    {
        for (int i = 0; i < Booths.Length; i++)
        {
            GameObject hero;
            if (_curSkin == RoleSkin.ARCHER_1 || _curSkin == RoleSkin.ARCHER_DEFAUL)
            {
                hero = GameManagers.Instance.GetArcherCharacter(i, _curSkin);
            }
            else
            {
                hero = GameManagers.Instance.GetWarriorCharacter(i, _curSkin);
            }
            Booths[i].ShowRole(hero);
        }

        for (int i = 0; i < Indicator.Length; i++)
        {
            if (i == skinIndex)
            {
                Indicator[i].sprite = Instantiate(SelectImg);
            }
            else
            {
                Indicator[i].sprite = Instantiate(UnSelectImg);
            }
        }

        SkinName.text = _skinName[skinIndex];
    }

    public void Hide()
    {
        Container.SetActive(false);
        UI.SetActive(false);
    }

    public void OnShowPreSkin()
    {
        if (skinIndex == 0)
        {
            return;
        }
        skinIndex--;
        _curSkin = _skins[skinIndex];
        RefreshBooth();
    }

    public void OnShowNextSkin()
    {
        if (skinIndex >= _skins.Length)
        {
            return;
        }
        skinIndex++;
        _curSkin = _skins[skinIndex];
        RefreshBooth();
    }


    public void OnSelect()
    {
        GameManagers.Instance.InvokeSkinChange(_skins[skinIndex]);
        UIManager.Instance.OnCloseSkinPageClick();
    }
}

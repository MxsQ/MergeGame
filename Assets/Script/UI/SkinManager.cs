using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    private static SkinManager _instance = null;

    [SerializeField] GameObject Container;
    [SerializeField] GameObject UI;
    [SerializeField] Booth[] Booths;
    [SerializeField] Image[] Indicator;

    [SerializeField] Sprite SelectImg;
    [SerializeField] Sprite UnSelectImg;

    [SerializeField] Sprite SkinInuseImg;
    [SerializeField] Sprite SkinSelectImg;

    [SerializeField] Text SkinName;
    [SerializeField] Image SelectBtn;
    [SerializeField] Text SelectBtnText;
    [SerializeField] Image RVForSkinBtn;

    private string[] _skinName = new string[] { "Padadin", "Elf Archer", "Assassin", "Berserker", "Undead Archer", "Cavalier" };

    public static RoleSkin[] Skins = new RoleSkin[]{
        RoleSkin.WARRIOR_DEFAUL,
        RoleSkin.ARCHER_DEFAUL,
        RoleSkin.WARRIOR_1,
        RoleSkin.WARRIOR_2,
        RoleSkin.ARCHER_1,
        RoleSkin.WARRIOR_3,
    };

    private RoleSkin _curSkin = RoleSkin.WARRIOR_DEFAUL;
    public int SkinIndex = 0;
    private char _skinStatus = '0';

    public static SkinManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Show()
    {
        Container.SetActive(true);
        UI.SetActive(true);
        RefreshBooth();
    }

    public void RefreshBooth()
    {
        var showIndex = 0;
        var record = GameManagers.Instance.PlayerRecored;
        if (SkinIndex == 1 || SkinIndex == 4)
        {
            showIndex = record.MaxArcherLevel;
        }
        else
        {
            showIndex = record.MaxWarriorLevel;
        }

        for (int i = 0; i <= showIndex; i++)
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

        for (int i = showIndex + 1; i < Booths.Length; i++)
        {
            Booths[i].Lock();
        }

        // change indicator
        for (int i = 0; i < Indicator.Length; i++)
        {
            if (i == SkinIndex)
            {
                Indicator[i].sprite = Instantiate(SelectImg);
            }
            else
            {
                Indicator[i].sprite = Instantiate(UnSelectImg);
            }
        }

        SkinName.text = _skinName[SkinIndex];
        _skinStatus = record.SkinStatus[SkinIndex];
        if (_skinStatus == '0')
        {
            SelectBtn.gameObject.SetActive(false);
            RVForSkinBtn.gameObject.SetActive(true);
        }
        else
        {
            SelectBtn.gameObject.SetActive(true);
            RVForSkinBtn.gameObject.SetActive(false);

            if (_curSkin == RoleSkin.ARCHER_1 || _curSkin == RoleSkin.ARCHER_DEFAUL)
            {
                // check archer status
                if (SkinIndex == record.ArcherSkinIndex)
                {
                    SelectBtn.sprite = GameObject.Instantiate<Sprite>(SkinInuseImg);
                    SelectBtnText.text = "InUse";
                }
                else
                {
                    SelectBtn.sprite = GameObject.Instantiate<Sprite>(SkinSelectImg);
                    SelectBtnText.text = "Select";
                }
            }
            else
            {
                // check warriror status
                if (SkinIndex == record.WarriorSkinIndex)
                {
                    SelectBtn.sprite = GameObject.Instantiate<Sprite>(SkinInuseImg);
                    SelectBtnText.text = "InUse";
                }
                else
                {
                    SelectBtn.sprite = GameObject.Instantiate<Sprite>(SkinSelectImg);
                    SelectBtnText.text = "Select";
                }
            }


        }
    }

    public void Hide()
    {
        Container.SetActive(false);
        UI.SetActive(false);
    }

    public void OnShowPreSkin()
    {
        AudioManager.Instance.PlayClick();
        if (SkinIndex == 0)
        {
            return;
        }
        SkinIndex--;
        _curSkin = Skins[SkinIndex];
        RefreshBooth();
    }

    public void OnShowNextSkin()
    {
        AudioManager.Instance.PlayClick();
        if (SkinIndex >= Skins.Length - 1)
        {
            return;
        }
        SkinIndex++;
        _curSkin = Skins[SkinIndex];
        RefreshBooth();
    }


    public void OnSelect()
    {
        GameManagers.Instance.InvokeSkinChange(Skins[SkinIndex]);
        UIManager.Instance.OnCloseSkinPageClick();
    }

    public void OnRVForSkin()
    {
        Ads.Instance.ShowRV(reward =>
        {
            if (reward)
            {
                var record = GameManagers.Instance.PlayerRecored;
                string newStatus = record.SkinStatus;
                newStatus = newStatus.Substring(0, SkinIndex) + "1" + newStatus.Substring(SkinIndex, newStatus.Length - SkinIndex - 1);
                record.SaveNewSkinStatus(newStatus);
                RefreshBooth();
            }
        });
    }
}

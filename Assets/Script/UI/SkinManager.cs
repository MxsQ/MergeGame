

using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    private static SkinManager _instance = null;

    [SerializeField] GameObject Container;
    [SerializeField] GameObject UI;
    [SerializeField] Booth[] Booths;
    [SerializeField] Image[] Indicator;
    [SerializeField] Transform Stage;
    [SerializeField] Transform Stub;
    [SerializeField] Transform StageLight;

    [SerializeField] Sprite SelectImg;
    [SerializeField] Sprite UnSelectImg;

    [SerializeField] Sprite SkinInuseImg;
    [SerializeField] Sprite SkinSelectImg;

    [SerializeField] Text SkinName;
    [SerializeField] Image SelectBtn;
    [SerializeField] Text SelectBtnText;
    [SerializeField] Image RVForSkinBtn;

    [SerializeField] GameObject LeftIndicator;
    [SerializeField] GameObject RightIndicator;
    [SerializeField] Image LeftOutline;
    [SerializeField] Image RightOutline;
    [SerializeField] Sprite[] OutlineImg;

    GameObject _curShowRole;


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
        var skinConfig = GameManagers.Instance.GetSkin(_curSkin);
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
            Booths[i].ShowRole(Instantiate(skinConfig.skin[i]));
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


        int leftOutlineIndex = SkinIndex - 1;
        int rightOutlineIndex = SkinIndex + 1;
        if (SkinIndex == 0)
        {
            leftOutlineIndex = Skins.Length - 1;
        }
        else if (SkinIndex == Skins.Length - 1)
        {
            rightOutlineIndex = 0;
        }
        LeftOutline.sprite = Instantiate<Sprite>(OutlineImg[leftOutlineIndex]);
        RightOutline.sprite = Instantiate<Sprite>(OutlineImg[rightOutlineIndex]);

        OnItemClick(0);
    }

    public void Hide()
    {
        Container.SetActive(false);
        UI.SetActive(false);
    }

    public void OnItemClick(int index)
    {

        GameObject role = null;
        if (_curSkin == RoleSkin.ARCHER_1 || _curSkin == RoleSkin.ARCHER_DEFAUL)
        {
            var record = GameManagers.Instance.PlayerRecored;
            if (record.MaxArcherLevel < index)
            {
                return;
            }

            role = GameManagers.Instance.GetArcherCharacter(index, _curSkin);

        }
        else
        {
            var record = GameManagers.Instance.PlayerRecored;
            if (record.MaxWarriorLevel < index)
            {
                return;
            }

            role = GameManagers.Instance.GetWarriorCharacter(index, _curSkin);
        }
        if (_curShowRole != null)
        {
            Destroy(_curShowRole);
        }


        Stage.position = new Vector3(Stub.position.x, Stub.position.y, 0);
        var roleT = role.gameObject.transform;
        roleT.parent = Stage;
        roleT.localScale = new Vector3(180, 180, 0);
        roleT.position = new Vector3(Stub.position.x, Stub.position.y - 40, 0);
        _curShowRole = role;

        StageLight.position = new Vector3(Stub.position.x, Stub.position.y, 0);
    }

    public void OnShowPreSkin()
    {
        AudioManager.Instance.PlayClick();
        SkinIndex--;
        if (SkinIndex < 0)
        {
            SkinIndex = Skins.Length - 1;
        }

        _curSkin = Skins[SkinIndex];
        RefreshBooth();
    }

    public void OnShowNextSkin()
    {
        AudioManager.Instance.PlayClick();
        SkinIndex++;
        if (SkinIndex > Skins.Length - 1)
        {
            SkinIndex = 0;
        }
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
        Ads.Instance.report("GetSkin_" + SkinIndex);
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

    [System.Serializable]
    public class Skin
    {
        [SerializeField] public RoleSkin ID;
        [SerializeField] public GameObject[] prefab;
        [SerializeField] public Sprite[] skin;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [SerializeField] GameObject Container;
    [SerializeField] GameObject UI;
    [SerializeField] Booth[] Booths;

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
            GameObject hero = GameManagers.Instance.GetWarriorCharacter(i);
            Booths[i].ShowRole(hero);
        }
    }

    public void Hide()
    {
        Container.SetActive(false);
        UI.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Booth : MonoBehaviour
{
    [SerializeField] Image Showing;
    [SerializeField] Image LockImg;

    public void ShowRole(Sprite role)
    {
        Showing.sprite = role;
        Showing.gameObject.SetActive(true);
        LockImg.gameObject.SetActive(false);
    }

    public void Lock()
    {
        LockImg.gameObject.SetActive(true);
        Showing.gameObject.SetActive(false);
    }

}

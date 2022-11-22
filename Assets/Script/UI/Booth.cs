using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Booth : MonoBehaviour
{
    [SerializeField] Image Showing;
    [SerializeField] Sprite LockImage;

    public void ShowRole(Sprite role)
    {
        Showing.sprite = role;
        Showing.gameObject.SetActive(true);
    }

    public void Lock()
    {
        Showing.sprite = null;
    }

}

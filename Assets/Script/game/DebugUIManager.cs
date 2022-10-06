using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUIManager : MonoBehaviour
{
    public void OnClearCacheClick()
    {
        GameManagers.Instance.PlayerRecored.ClearCache();
    }
}

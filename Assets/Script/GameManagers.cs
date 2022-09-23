using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    private static GameManagers _instance = null;

    [SerializeField] public GlobalConfigScriptableObject Config;

    public static GameManagers Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForCompileTest : MonoBehaviour
{

    [SerializeField] GameObject[] Prefabs;
    [SerializeField] Material[] meshes;
    [SerializeField] Shader[] shaders;

    private void Awake()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i] == null)
            {
                Debug.Log("dismiss material: " + i);
            }
            else
            {
                Debug.Log("has material: " + i);
            }
        }

        for (int i = 0; i < shaders.Length; i++)
        {
            if (shaders[i] == null)
            {
                Debug.Log("shaders dissmiss: " + i);
            }
            else
            {
                Debug.Log("shaders has: " + i);
            }
        }
    }
}

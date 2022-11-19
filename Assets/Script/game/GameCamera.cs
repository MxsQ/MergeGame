using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * solotion by : http://t.zoukankan.com/programmer-blog-p-4418597.html
 */
public class GameCamera : MonoBehaviour
{
    float hopeHeight = 1920;
    float hopeWidth = 1080;

    // Start is called before the first frame update
    void Start()
    {
        float hopeSzie = hopeHeight / 2;
        float screentHeight = Screen.height;
        float orthographicSize = this.GetComponent<Camera>().orthographicSize;
        float aspectRatio = Screen.width * 1.0f / Screen.height;
        float cameraWidth = orthographicSize * aspectRatio;
        Debug.Log(
            "hopeOrthographicSize=" + hopeSzie
            + "  ScreentHight=" + screentHeight
            + "   orthographicSize=" + orthographicSize
            + " aspectRatio=" + aspectRatio
            + " cameraWidth=" + cameraWidth);


        if (cameraWidth < hopeWidth / 2)
        {
            orthographicSize = hopeWidth / aspectRatio / 2;
            Debug.Log("new orthographicSize=" + orthographicSize);
            this.GetComponent<Camera>().orthographicSize = orthographicSize;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugIcon : MonoBehaviour
{

    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        if (PlayerPrefs.GetInt("Debug") == 1) image.enabled = true;
        else image.enabled = false;
    }
}

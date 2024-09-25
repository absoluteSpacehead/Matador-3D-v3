using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    GameObject left;
    RectTransform LeftTrans;
    GameObject right;
    RectTransform RightTrans;
    public bool menuOpen;

    // Start is called before the first frame update
    void Start()
    {
        left = GameObject.Find("MenuLeft");
        LeftTrans = left.GetComponent<RectTransform>();
        right = GameObject.Find("MenuRight");
        RightTrans = right.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) menuOpen = !menuOpen;
    }

    private void FixedUpdate()
    {
        if (menuOpen)
        {
            if (LeftTrans.localPosition.x < -120.0001)
            {
                LeftTrans.localPosition = Vector3.Lerp(LeftTrans.localPosition, new Vector3(-120, 0, 0), 0.066f);
                RightTrans.localPosition = Vector3.Lerp(RightTrans.localPosition, new Vector3(120, 0, 0), 0.066f);
            }
        }
        else if (LeftTrans.localPosition.x > -199.9998)
        {
            LeftTrans.localPosition = Vector3.Lerp(LeftTrans.localPosition, new Vector3(-200, 0, 0), 0.066f);
            RightTrans.localPosition = Vector3.Lerp(RightTrans.localPosition, new Vector3(200, 0, 0), 0.066f);
        }
    }
}

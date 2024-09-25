using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressStart : MonoBehaviour
{

    Animator CameraAnimator;
    Animator LogoAnimator;
    RectTransform TextBoxTrans;
    public bool PressedStart = false;
    AudioSource sound;
    RectTransform trans;
    bool ReadyToMove = false;

    // Start is called before the first frame update
    void Start()
    {
        //set things
        CameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
        LogoAnimator = GameObject.Find("Logo").GetComponent<Animator>();
        TextBoxTrans = GameObject.Find("TextBox").GetComponent<RectTransform>();
        sound = GetComponent<AudioSource>();
        trans = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && !PressedStart)
        {
            PressedStart = true;
            CameraAnimator.Play("LookDown");
            LogoAnimator.Play("MoveUp");
            trans.localPosition = new Vector3(trans.localPosition.x - 300, trans.localPosition.y - 300);
            sound.Play();
            StartCoroutine(WaitFor());
        }
    }

    void FixedUpdate()
    {
        if (ReadyToMove && TextBoxTrans.localPosition.y > 0.0001)
        {
            TextBoxTrans.localPosition = Vector3.Lerp(TextBoxTrans.localPosition, new Vector3(TextBoxTrans.localPosition.x, 0, 0), 0.066f);
        }
    }

    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(1);
        ReadyToMove = true;
    }
}
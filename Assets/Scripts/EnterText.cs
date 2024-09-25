using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnterText : MonoBehaviour
{

    public string Name;
    public int x = 1;
    public int y = -1;
    RectTransform trans;
    TextMeshProUGUI text;
    AudioSource sound1;
    AudioSource sound2;
    AudioSource sound3;
    AudioSource sound4;
    public AudioClip sound2clip;
    public AudioClip sound3clip;
    public AudioClip sound5clip;
    public Image image;
    public Image DarkBoxImage;
    public string test;
    public float test2;
    Animator animator;
    TextMeshProUGUI FileText;
    AudioSource aniamtor2;
    RectTransform FileParentTrans;
    Animator CameraAnimator;

    public bool NameBoxReady = false;
    public bool AlphaReady = false;
    public bool VolumeReady = false;

    fade fadeScript;

    readonly string[,] characters = new string[,] { { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" }, { "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" }, { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m" }, { "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" }, { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", ".", "!" }, { "?", ",", ";", ":", '"'.ToString(), "'", "(", ")", "/", "-", "+", "dummy", " " } };

    RectTransform LightBoxTrans;
    RectTransform DarkBoxTrans;


    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<RectTransform>();
        text = GameObject.Find("FileNameText").GetComponent<TextMeshProUGUI>();
        LightBoxTrans = GameObject.Find("TextBox").GetComponent<RectTransform>();
        DarkBoxTrans = GameObject.Find("TextBoxDARK").GetComponent<RectTransform>();
        image = GetComponent<Image>();
        DarkBoxImage = GameObject.Find("TextBoxDARK").GetComponent<Image>();
        animator = GameObject.Find("TextBox").GetComponent<Animator>();
        animator.enabled = false;
        sound1 = GetComponent<AudioSource>();
        sound2 = gameObject.AddComponent<AudioSource>();
        sound2.playOnAwake = false;
        sound2.clip = sound2clip;
        sound3 = gameObject.AddComponent<AudioSource>();
        sound3.playOnAwake = false;
        sound3.clip = sound3clip;
        sound4 = GameObject.Find("Press Start!").GetComponent<AudioSource>();
        FileText = GameObject.Find("FileText").GetComponent<TextMeshProUGUI>();
        aniamtor2 = GameObject.Find("FileParent").GetComponent<AudioSource>();
        CameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
        fadeScript = GameObject.Find("fadeout").GetComponent<fade>();
        FileParentTrans = aniamtor2.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        trans.localPosition = new Vector3(x * 14 - 99.5f, y * 18.7f + 56); //set position

        //Move
        if (LightBoxTrans.localPosition.y < 8 && !AlphaReady) //make sure its fully faded in
        {

            if (Input.GetKeyDown(KeyCode.LeftArrow) && x != 1)
            {
                --x; //left
                sound1.Play(); //play whom sound
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) && x != 13)
            {
                ++x; //right
                sound1.Play();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && y != -1)
            {
                ++y; //up
                sound1.Play();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && y != -6)
            {
                --y; //down
                sound1.Play();
            }

            test = characters[Mathf.Abs(y) - 1, x - 1];
            test2 = text.renderedWidth;

            //enter character
            if(Input.GetKeyDown(KeyCode.Z))
            {
                if(characters[Mathf.Abs(y) - 1, x - 1] != "dummy")
                {
                    if(text.renderedWidth < 80) text.text += characters[Mathf.Abs(y) - 1, x - 1];
                } else text.text = text.text.Substring(0, text.text.Length - 1);
            }

            if (Input.GetKeyDown(KeyCode.Return) && !animator.enabled && !AlphaReady)
            {
                //finish name
                if (string.IsNullOrWhiteSpace(text.text) || text.text.ToLower() == "matador") //is it empty? (also cant have two matadors)
                {
                    if (!animator.enabled)
                    {
                        animator.enabled = true;
                        animator.Play("shakenamebox", 0, 0);
                        StartCoroutine(WaitFor(0.5f, "box"));
                        sound3.Play();
                    }
                } else
                {
                    //do animations and the save file things pop up and MATADOR exists
                    AlphaReady = true;
                    StartCoroutine(WaitFor(1, "move"));
                    CameraAnimator.Play("MATADOR");
                    sound4.Play();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && LightBoxTrans.localPosition.x < -315 && !VolumeReady)
        {
            sound2.Play();
            fadeScript.enabled = true;
            VolumeReady = true;
        }


        FileText.text = text.text;
    }

    void LateUpdate()
    {
        DarkBoxTrans.position = LightBoxTrans.position;
    }

    void FixedUpdate()
    {
        if (NameBoxReady && LightBoxTrans.localPosition.x > -319.0001)
        {
            LightBoxTrans.localPosition = Vector3.Lerp(LightBoxTrans.localPosition, new Vector3(-320, 0, 0), 0.066f);
            FileParentTrans.localPosition = Vector3.Lerp(FileParentTrans.localPosition, new Vector3(0, 0, 0), 0.066f);
        }

        if (AlphaReady && image.color.a > 0.0001)
        {
            image.color = Color.Lerp(image.color, new Color(1, 1, 1, 0), 0.066f);
            DarkBoxImage.color = image.color;
        }

        if(VolumeReady && aniamtor2.volume > 0)
        {
            aniamtor2.volume -= 0.025f;
        }
    }

    IEnumerator WaitFor(float seconds, string method)
    {
        yield return new WaitForSeconds(seconds);
        
        switch(method)
        {
            case "box":
                animator.enabled = false;
                break;
            case "move":
                NameBoxReady = true;
                break;
        }
    }
}

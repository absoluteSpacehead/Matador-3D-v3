using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextBox : MonoBehaviour
{

    public string currentText = "";
    public string text;
    public int currentLetter = 0;
    public string letter;
    Image image;
    Image arrow;
    TextMeshProUGUI textBox;
    AudioSource src1;
    public AudioSource src2;
    AudioSource src3;
    public AudioClip clip2;
    public AudioClip clip3;
    public bool show;
    bool waiting = false;
    bool snap = false;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        arrow = GameObject.Find("TextBoxArrow").GetComponent<Image>();
        textBox = GameObject.Find("TextBoxText").GetComponent<TextMeshProUGUI>();
        src1 = GetComponent<AudioSource>();
        src2 = gameObject.AddComponent<AudioSource>();
        src2.playOnAwake = false;
        src2.clip = clip2;
        src2.reverbZoneMix = 0;
        src3 = gameObject.AddComponent<AudioSource>();
        src3.playOnAwake = false;
        src3.clip = clip3;
        src3.reverbZoneMix = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!show) //if not showing
        {
            if (image.color.a != 0) //if its still visible
            {
                image.color = new Color(1, 1, 1, 0); //hide
                textBox.color = new Color(0, 0, 0, 0); //every
                arrow.color = new Color(1, 1, 1, 0); //thing
            }

            if (src1.volume != 0) //make sure its not playing sound
            {
                src1.volume -= 0.1f;
            }

            currentLetter = 0; //reset these so that once it shows again it doesnt fuck up
            currentText = "";
            letter = "";
            snap = false;
        } else //if showing, other scripts can use this variable to show text
        {

            if(!snap) //play snap sound
            {
                snap = true;
                src2.Play();
            }

            if(currentText != text) letter = text[currentLetter].ToString(); //set the letter to the current letter

            if (image.color.a != 1) //set image and text to visible
            {
                image.color = new Color(1, 1, 1, 1);
                textBox.color = new Color(0, 0, 0, 1);
            }

            if (currentText != text) //if its not finished
            {
                currentText = text.Substring(0, currentLetter + 1); //set currenttext even though this is kinda pointless
                textBox.maxVisibleCharacters = currentLetter + 1; //text is all in the text box at once, it just changes the max visible characters so it doesnt wrap a word halfway through

                switch(letter)
                {
                    case ".": //if its
                    case "!": //full
                    case "?": //punctuation
                        if (!waiting && currentText != text) StartCoroutine(WaitFor(false)); //wait for like half a second
                        break;
                    case ",": //if its a comma
                        if (!waiting && currentText != text) StartCoroutine(WaitFor(true)); //wait for like a quarter of a second
                        break;
                    default: //if its none of the above, just add one
                        ++currentLetter;
                        break;
                }

                if(src1.volume != 1 && !waiting) //if youre not waiting for some punctuation and volume isnt max
                {
                    src1.volume += 0.1f; //up the volume slightly
                } else if (waiting && src1.volume != 0) //if youre waiting and its not min volume
                {
                    src1.volume -= 0.1f; //lower the volume slightly
                }
            } else //if it IS finished
            {
                arrow.color = new Color(1, 1, 1, 1); //show the arrow
                if (src1.volume != 0) //lower the volume
                {
                    src1.volume -= 0.1f;
                }
            }
            textBox.text = text; //this happens regardless of if its finished
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && show) //if it IS showing and you press Z
        {
            if (currentText != text) //if its not finished
            {
                currentText = text;
                textBox.maxVisibleCharacters = text.Length; //skip to the end and finish it
            }
            else //if it IS finished
            {
                src3.Play(); //play the clap sound and stop showing it
                show = false;
            }
        }
    }

    IEnumerator WaitFor(bool comma) //bool is to determine if it should wait 0.5f or 0.25f
    {
        waiting = true; //set waiting to true so that we dont get like 50 WaitFors at once
        if (!comma) yield return new WaitForSeconds(0.5f); //if its not a comma wait the full time, 0.5f
        else yield return new WaitForSeconds(0.25f); //if not, wait 0.25f
        ++currentLetter; //add one to the current letter
        waiting = false; //set waiting to false so that we can wait again
    }
}

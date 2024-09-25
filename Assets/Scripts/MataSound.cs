using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MataSound : MonoBehaviour
{

    AudioSource src;
    public AudioClip clip;
    bool played = false;
    TextBox text;
    bool said = false;
    fade fade;

    // Start is called before the first frame update
    void Start()
    {
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.clip = clip;
        text = GameObject.Find("TextBox").GetComponent<TextBox>();
        fade = GameObject.Find("fadeout").GetComponent<fade>();
    }

    // Update is called once per frame
    void Update()
    {

        if(!said)
        {
            text.show = true;
            text.text = "You're here.\nHe's waiting for you.\nJust to your right.";
            said = true;
        }

        if(PlayerPrefs.GetInt("Debug") == 1)
        {
            if (Input.GetKeyDown(KeyCode.L)) transform.position = new Vector3(6, 0.44f, 0);
            if(Input.GetKeyDown(KeyCode.K))
            {
                transform.position = new Vector3(4, 0.44f, 0);
                played = true;
            }
        }

        if (transform.position.x < 5 && !played && !text.show)
        {
            src.Play();
            played = true;
            text.show = true;
            text.text = "You found him.\nCongratulations.";
        }

        /*if (played && !text.show)
        {
            fade.nextscene = 2;
            fade.fadeout = true;
            fade.enabled = true;
        }*/
    }

    private void FixedUpdate()
    {
        /*if (played && !text.show)
        {
            if (src.volume > 0)
            {
                src.volume -= 0.025f;
            }
        }*/
    }
}

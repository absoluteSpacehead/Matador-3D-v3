using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatadorMusicMenu : MonoBehaviour
{

    RectTransform trans;
    AudioSource audiosrc;
    AudioSource audiosrc2;
    public AudioClip clip;
    bool Playing;

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<RectTransform>();
        audiosrc = GetComponent<AudioSource>();
        audiosrc2 = gameObject.AddComponent<AudioSource>();
        audiosrc2.playOnAwake = false;
        audiosrc2.clip = clip;
    }

    // Update is called once per frame
    void Update()
    {
        if(trans.localPosition.x < 50 && !Playing)
        {
            Playing = true;
            audiosrc.Play();
        }

        if (trans.localPosition.x < 50 && audiosrc.volume == 1)
        {
            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                audiosrc2.Play();
            }
        }
    }
}

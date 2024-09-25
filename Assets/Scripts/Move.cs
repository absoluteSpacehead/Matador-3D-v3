using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    public float speed;
    AudioSource src;
    Menu menu;

    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        menu = GameObject.Find("menu").GetComponent<Menu>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!menu.menuOpen)
        {
            transform.Translate(new Vector3((0.05f * Input.GetAxisRaw("Horizontal")), 0, (0.05f * Input.GetAxisRaw("Vertical"))));

            /*if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                if (speed < 1)
                {
                    speed += 0.2f;
                }
                else
                {
                    speed = 1;
                }
            }
            else
            {
                if (speed > 0)
                {
                    speed -= 0.2f;
                }
                else
                {
                    speed = 0;
                }
            }*/

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                if (src.volume != 1)
                {
                    src.volume += 0.2f;
                }
            }
            else
            {
                if (src.volume != 0)
                {
                    src.volume -= 0.2f;
                }
            }
        } else
        {
            if (src.volume != 0)
            {
                src.volume -= 0.2f;
            }
        }
    }
}

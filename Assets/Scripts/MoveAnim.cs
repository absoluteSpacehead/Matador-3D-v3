using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnim : MonoBehaviour
{

    Animator animator;
    SpriteRenderer renderer;
    public Sprite image;
    Menu menu;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        menu = GameObject.Find("menu").GetComponent<Menu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!menu.menuOpen)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                if (!animator.enabled) animator.enabled = true;
            }
            else
            {
                animator.enabled = false;
                renderer.sprite = image;
            }
        } else
        {
            animator.enabled = false;
            renderer.sprite = image;
        }
    }
}

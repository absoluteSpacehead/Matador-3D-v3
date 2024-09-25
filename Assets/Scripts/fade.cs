using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class fade : MonoBehaviour
{

    public bool fadeout;
    public float alpha;
    public int nextscene;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        if (fadeout)
        {
            if (alpha != 1)
            {
                alpha += 0.02f;
            }

            if (alpha == 1) SceneManager.LoadScene(nextscene);
        }
        else
        {
            if (alpha != 0)
            {
                alpha -= 0.02f;
            }

            if (alpha == 0) this.enabled = false;
        }

        alpha = Mathf.Clamp(alpha, 0, 1);

        image.color = new Color(0, 0, 0, alpha);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreTextBoxMataRoom : MonoBehaviour
{

    TextBox text;
    Menu menu;
    bool stayhere;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextBox>();
        //StartCoroutine(WaitFor(60, "stillhere"));
        menu = GameObject.Find("menu").GetComponent<Menu>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!stayhere)
        {
            stayhere = true;
            text.show = true;
            text.text = "There. Now stay here.";
        }

        if(PlayerPrefs.GetInt("Debug") == 1)
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                StopCoroutine(WaitFor(60, "stillhere"));
                StartCoroutine(WaitFor(1, "stillhere"));
            }
        }
    }

    IEnumerator WaitFor(int time, string method)
    {
        yield return new WaitForSeconds(time);
        
        switch(method)
        {
            case "stillhere":
                text.show = true;
                text.text = "Wow, you're still here?\nYou must be dedicated.\nCheck your menu.";
                menu.enabled = true;
                break;
        }
    }
}

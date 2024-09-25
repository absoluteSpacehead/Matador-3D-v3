using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
    public bool DoFadeOut;
    private float _alpha;
    [SerializeField]
    private int _nextScene;
    private Image _image;

    // Start is called before the first frame update
    private void Start()
    {
        _image = GetComponent<Image>();

        if (!DoFadeOut)
            _alpha = 1.0f;
    }

    private void Update()
    {
        if (DoFadeOut)
        {
            _alpha += Time.deltaTime;

            if (_alpha >= 1.0f)
                SceneManager.LoadScene(_nextScene);
        }
        else
        {
            _alpha -= Time.deltaTime;

            if (_alpha <= 0.0f) enabled = false;
        }

        _alpha = Mathf.Clamp(_alpha, 0.0f, 1.0f);

        _image.color = new Color(0.0f, 0.0f, 0.0f, _alpha);
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 2024 note: what the fuck is this man
[RequireComponent(typeof(Image), typeof(AudioSource))]
public class EnterText : MonoBehaviour
{
    private string _currentName;
    private int _x = 1;
    private int _y = -1;
    private RectTransform _transform;
    private TextMeshProUGUI _text;
    private AudioSource _audioSource1;
    private AudioSource _audioSource2;
    private AudioSource _audioSource3;
    private AudioSource _audioSource4;
    [SerializeField]
    private AudioClip _audioClip2;
    [SerializeField]
    private AudioClip _audioClip3;
    private Image _image;
    private Image _darkImage;
    private Animator _animator;
    private TextMeshProUGUI _fileText;
    private AudioSource _audioSource5;
    private RectTransform _fileParentTransform;
    private Animator _cameraAnimator;

    private bool _nameBoxReady = false;
    private bool _alphaReady = false;
    private bool _volumeReady = false;

    private Fader _fader;

    // ??????
    readonly string[,] characters = new string[,] { { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" }, { "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" }, { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m" }, { "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" }, { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", ".", "!" }, { "?", ",", ";", ":", '"'.ToString(), "'", "(", ")", "/", "-", "+", "dummy", " " } };

    private RectTransform _lightBoxTransform;
    private RectTransform _darkBoxTransform;


    private void Start()
    {
        // 2024 note: im not even gonna begin to sort these. what the fuck man.
        _transform = GetComponent<RectTransform>();
        _text = GameObject.Find("FileNameText").GetComponent<TextMeshProUGUI>();
        _lightBoxTransform = GameObject.Find("TextBox").GetComponent<RectTransform>();
        _darkBoxTransform = GameObject.Find("TextBoxDARK").GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _darkImage = _darkBoxTransform.GetComponent<Image>();
        _animator = _lightBoxTransform.GetComponent<Animator>();
        _animator.enabled = false;
        _audioSource1 = GetComponent<AudioSource>();
        _audioSource2 = gameObject.AddComponent<AudioSource>();
        _audioSource2.playOnAwake = false;
        _audioSource2.clip = _audioClip2;
        _audioSource3 = gameObject.AddComponent<AudioSource>();
        _audioSource3.playOnAwake = false;
        _audioSource3.clip = _audioClip3;
        _audioSource4 = GameObject.Find("PressStart").GetComponent<AudioSource>();
        _fileText = GameObject.Find("FileText").GetComponent<TextMeshProUGUI>();
        _audioSource5 = GameObject.Find("FileParent").GetComponent<AudioSource>();
        _cameraAnimator = Camera.main.GetComponent<Animator>();
        _fader = GameObject.Find("fadeout").GetComponent<Fader>();
        _fileParentTransform = _audioSource5.gameObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        _transform.localPosition = new Vector3(_x * 14.0f - 99.5f, _y * 18.7f + 56.0f); //set position

        //Move
        if (_lightBoxTransform.localPosition.y < 8.0f && !_alphaReady) //make sure its fully faded in
        {
            float horizontal = Input.GetButtonDown("Left") ? -1.0f : (Input.GetButtonDown("Right") ? 1.0f : 0.0f);
            float vertical = Input.GetButtonDown("Down") ? 1.0f : (Input.GetButtonDown("Up") ? -1.0f : 0.0f);
            if (horizontal != 0.0f)
            {
                if (horizontal < 0.0f && _x == 1) return;
                else if (horizontal > 0.0f && _x == 13) return;

                _x += horizontal > 0.0f ? 1 : -1;
                _audioSource1.PlayOneShot(_audioSource1.clip);
            }
            else if (vertical != 0.0f)
            {
                if (vertical < 0.0f && _y == -1) return;
                else if (vertical > 0.0f && _y == -6) return;

                _y += vertical > 0.0f ? -1 : 1;
                _audioSource1.PlayOneShot(_audioSource1.clip);
            }

            //enter character
            if(Input.GetButtonDown("Select"))
            {
                if(characters[Mathf.Abs(_y) - 1, _x - 1] != "dummy")
                {
                    if(_text.renderedWidth < 80)
                        _text.text += characters[Mathf.Abs(_y) - 1, _x - 1];
                }
                else
                    _text.text = _text.text.Substring(0, _text.text.Length - 1);

                _fileText.text = _text.text;
            }

            if (Input.GetButtonDown("Start") && !_animator.enabled && !_alphaReady)
            {
                //finish name
                if (string.IsNullOrWhiteSpace(_text.text) || _text.text.ToLower().Replace(" ", "") == "matador") //is it empty? (also cant have two matadors)
                {
                    if (!_animator.enabled)
                    {
                        _animator.enabled = true;
                        _animator.Play("ShakeNameBox", 0, 0);
                        StartCoroutine(WaitFor(0.5f, "box"));
                        _audioSource3.Play();
                    }
                } else
                {
                    //do animations and the save file things pop up and MATADOR exists
                    _alphaReady = true;
                    StartCoroutine(WaitFor(1, "move"));
                    _cameraAnimator.Play("MATADOR");
                    _audioSource4.Play();
                }
            }
        }

        if (Input.GetButtonDown("Start") && _lightBoxTransform.localPosition.x < -315.0f && !_volumeReady)
        {
            _audioSource2.Play();
            _fader.enabled = true;
            _volumeReady = true;
        }

        if (_nameBoxReady && _lightBoxTransform.localPosition.x > -319.0001f)
        {
            _lightBoxTransform.localPosition = Vector3.Lerp(_lightBoxTransform.localPosition, new Vector3(-320.0f, 0.0f, 0.0f), 3.3f * Time.deltaTime);
            _fileParentTransform.localPosition = Vector3.Lerp(_fileParentTransform.localPosition, new Vector3(0.0f, 0.0f, 0.0f), 3.3f * Time.deltaTime);
        }

        if (_alphaReady && _image.color.a > 0.0001f)
        {
            _image.color = Color.Lerp(_image.color, new Color(1.0f, 1.0f, 1.0f, 0.0f), 3.3f * Time.deltaTime);
            _darkImage.color = _image.color;
        }

        if(_volumeReady && _audioSource5.volume > 0.0f)
        {
            _audioSource5.volume -= 1.25f * Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        _darkBoxTransform.position = _lightBoxTransform.position;
    }

    private IEnumerator WaitFor(float seconds, string method)
    {
        yield return new WaitForSeconds(seconds);
        
        switch(method)
        {
            case "box":
                _animator.enabled = false;
                break;
            case "move":
                _nameBoxReady = true;
                break;
        }
    }
}

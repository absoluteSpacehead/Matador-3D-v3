using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource), typeof(RectTransform))]
public class PressStart : MonoBehaviour
{
    // Components
    private Animator _cameraAnimator;
    private Animator _logoAnimator;
    private RectTransform _nameBoxTransform;
    private AudioSource _audioSource;
    private RectTransform _transform;

    public bool HasPressedStart;
    bool ReadyToMove;

    private void Start()
    {
        _cameraAnimator = Camera.main.GetComponent<Animator>();
        _logoAnimator = GameObject.Find("Logo").GetComponent<Animator>();
        _nameBoxTransform = GameObject.Find("TextBox").GetComponent<RectTransform>();
        _audioSource = GetComponent<AudioSource>();
        _transform = GetComponent<RectTransform>();

        // this probably needs a better place but im lazy
        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Start") && !HasPressedStart)
        {
            HasPressedStart = true;
            _cameraAnimator.Play("LookDown");
            _logoAnimator.Play("MoveUp");
            _transform.localPosition = new Vector3(_transform.localPosition.x - 300, _transform.localPosition.y - 300);
            _audioSource.Play();
            StartCoroutine(WaitFor());
        }

        if (ReadyToMove && _nameBoxTransform.localPosition.y > 0.0001)
            _nameBoxTransform.localPosition = Vector3.Lerp(_nameBoxTransform.localPosition, Vector3.right * _nameBoxTransform.localPosition.x, 3.3f * Time.deltaTime);
    }

    private IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(1);
        ReadyToMove = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(AudioSource))]
public class MatadorMusicMenu : MonoBehaviour
{
    private RectTransform _transform;
    private AudioSource _audioSource;
    private AudioSource _audioSource2;
    public AudioClip _audioClip;
    private bool _isPlaying;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<RectTransform>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource2 = gameObject.AddComponent<AudioSource>();
        _audioSource2.playOnAwake = false;
        _audioSource2.clip = _audioClip;
    }

    // Update is called once per frame
    void Update()
    {
        if(_transform.localPosition.x < 50 && !_isPlaying)
        {
            _isPlaying = true;
            _audioSource.Play();
        }

        if (_transform.localPosition.x < 50 && _audioSource.volume == 1)
        {
            if(Input.GetAxisRaw("Vertical") != 0.0f)
            {
                _audioSource2.PlayOneShot(_audioSource2.clip);
            }
        }
    }
}

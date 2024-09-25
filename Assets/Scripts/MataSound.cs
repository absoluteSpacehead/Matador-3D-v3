using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MataSound : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _clip;
    private bool _hasPlayedSound = false;

    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.clip = _clip;
    }

    private void Update()
    {
        if (transform.position.x < 5.0f && !_hasPlayedSound)
        {
            _audioSource.Play();
            _hasPlayedSound = true;
        }
    }
}

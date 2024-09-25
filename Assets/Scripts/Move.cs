using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Move : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized * 2.5f * Time.deltaTime);

        if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
        {
            if (_audioSource.volume != 1.0f)
                _audioSource.volume = Mathf.Clamp(_audioSource.volume + (10.0f * Time.deltaTime), 0.0f, 1.0f);
        }
        else
        {
            if (_audioSource.volume != 0.0f)
                _audioSource.volume = Mathf.Clamp(_audioSource.volume - (10.0f * Time.deltaTime), 0.0f, 1.0f);
        }
    }
}

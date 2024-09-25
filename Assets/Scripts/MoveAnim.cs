using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class MoveAnim : MonoBehaviour
{

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    public Sprite StandImage;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
        {
            if (!_animator.enabled)
                _animator.enabled = true;
        }
        else
        {
            _animator.enabled = false;
            _spriteRenderer.sprite = StandImage;
        }
    }
}

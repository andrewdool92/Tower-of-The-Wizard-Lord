using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallDropable : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] float animationDelay = 0.5f;
    [SerializeField] float delay = 1.3f;
    [SerializeField] float offset = 100;
    [SerializeField] Rigidbody2D _rigidbody;

    private float _timer;
    public event Action<float> onPitfall;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                transform.position += Vector3.down * (offset + 1);
                _animator.SetBool("falling", false);
            }
        }
    }

    public void drop()
    {
        _timer = animationDelay;
        _animator.SetBool("falling", true);
        _rigidbody.velocity = Vector3.zero;
        onPitfall?.Invoke(delay);
    }
        
}

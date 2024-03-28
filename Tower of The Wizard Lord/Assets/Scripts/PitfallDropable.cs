using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallDropable : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] float animationDelay;
    [SerializeField] float delay;
    [SerializeField] float offset;

    private float _timer;
    public event Action<float> onPitfall;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                transform.position += Vector3.down * offset;
                _animator.SetBool("falling", false);
            }
        }
    }

    public void drop()
    {
        _timer = animationDelay;
        _animator.SetBool("falling", true);
        onPitfall?.Invoke(delay);
    }
        
}

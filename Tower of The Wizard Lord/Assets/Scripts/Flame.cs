using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [SerializeField] public bool onFire;
    [SerializeField] private AudioClip igniteAudio;
    private Animator _animator;
    private ParticleSystem _sparks;

    public delegate void StateChange(bool state);
    public StateChange stateChange;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _sparks = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if ( !onFire && collision.gameObject.CompareTag("fire"))
        {
            _animator.SetBool("on_fire", true);
            onFire = true;
            _sparks.Play();

            stateChange?.Invoke(true);
        }
    }
}

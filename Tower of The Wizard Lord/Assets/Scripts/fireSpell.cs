using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : MonoBehaviour
{
    [SerializeField] float positionOffset;

    private Animator _animator;
    private Collider2D _collider;
    private Transform _transform;

    private bool _active;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _transform = GetComponent<Transform>();

        _active = false;
        _collider.enabled = false;
        _animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void cast(Vector2 position, Vector2 direction)
    {
        _transform.position = position + direction * positionOffset;
        _active = true;


    }
}

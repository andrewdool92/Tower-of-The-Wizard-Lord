using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float acceleration;
    [SerializeField] private float topSpeed;
    private float _timer;

    private Vector2 _moveDirection;
    private Rigidbody2D _body;
    private Animator _animator;

    private delegate void Movement();
    Movement _move;


    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
//        _animator = GetComponent<Animator>();
        _animator = GetComponentInChildren<Animator>();

        _move = mobile;

        inputReader.MoveEvent += handleMove;
        inputReader.SpellcastEvent += handleSpellcast;
        inputReader.SpellcastCancelledEvent += handleSpellcastCancelled;
    }

    private void OnDestroy()
    {
        inputReader.MoveEvent -= handleMove;
        inputReader.SpellcastEvent -= handleSpellcast;
        inputReader.SpellcastCancelledEvent -= handleSpellcastCancelled;
    }

    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;
        _animator.SetFloat("HoldTime", _timer);
    }

    void FixedUpdate()
    {
        _move();
    }

    void handleMove(Vector2 input)
    {
        _moveDirection = input;
    }

    void handleSpellcast()
    {
        _timer = 0;
        _move = rooted;
        _animator.SetBool("Running", false);
        _animator.SetBool("Casting", true);
        _animator.SetFloat("HoldTime", _timer);
    }

    void handleSpellcastCancelled()
    {
        _move = mobile;
        _animator.SetBool("Casting", false);
    }

    void mobile()
    {
        if (_moveDirection != Vector2.zero)
        {
            updateAnimatorVector();
            _animator.SetBool("Running", true);
            
            _body.AddForce(_moveDirection * acceleration);
            _body.velocity = Vector2.ClampMagnitude(_body.velocity, topSpeed);
        }
        else
        {
            _animator.SetBool("Running", false);
        }

        _animator.SetFloat("Speed", _body.velocity.magnitude);
    }

    void rooted()
    {
        if (_moveDirection != Vector2.zero)
        {
            updateAnimatorVector();
        }
    }

    private void updateAnimatorVector()
    {
        _animator.SetFloat("AnimMoveX", _moveDirection.x);
        _animator.SetFloat("AnimMoveY", _moveDirection.y);
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float acceleration;
    [SerializeField] private float topSpeed;

    private Vector2 _moveDirection;
    private Rigidbody2D _body;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        inputReader.MoveEvent += handleMove;
    }

    private void OnDestroy()
    {
        inputReader.MoveEvent -= handleMove;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
    }

    void handleMove(Vector2 input)
    {
        _moveDirection = input;
    }

    void move()
    {
        if (_moveDirection != Vector2.zero)
        {
            _animator.SetFloat("AnimMoveX", _moveDirection.x);
            _animator.SetFloat("AnimMoveY", _moveDirection.y);
            _animator.SetBool("Running", true);
            
            _body.AddForce(_moveDirection * acceleration * Time.deltaTime);
            _body.velocity = Vector2.ClampMagnitude(_body.velocity, topSpeed);
        }
        else
        {
            _animator.SetBool("Running", false);
        }

        _animator.SetFloat("Speed", _body.velocity.magnitude);
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float acceleration;
    [SerializeField] private float topSpeed;
    [SerializeField] private float spellCooldown;
    [SerializeField] private Spell defaultSpell;
    private Spell _spell;

    private Vector2 _moveDirection;
    private Vector2 _facingDirection;
    private Rigidbody2D _body;
    private Animator _animator;

    private delegate void Movement();
    Movement _move;

    private delegate void OnAction();
    OnAction _action;
    private float _timer;


    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _move = mobile;
        _action = noAction;

        inputReader.MoveEvent += handleMove;
        inputReader.SpellcastEvent += handleSpellcast;
        inputReader.SpellcastCancelledEvent += handleSpellcastCancelled;

        _spell = Instantiate(defaultSpell);
        _facingDirection = Vector2.down;
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
        _action();
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
        _action = chargingSpell;
        _animator.SetBool("Running", false);
        _animator.SetBool("Casting", true);
        _animator.SetFloat("HoldTime", _timer);
    }

    void handleSpellcastCancelled()
    {
        _animator.SetBool("Casting", false);
        
        if (_timer < 0.2)
        {
            _move = mobile;
            _action = noAction;
        }
        else
        {
            _action = releaseSpell;
            _move = noAction;
            _spell.activate(transform.position, _facingDirection);
        }
        
        _timer = 0;
    }

    void mobile()
    {
        if (_moveDirection != Vector2.zero)
        {
            updateFacing();
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
            updateFacing();
            updateAnimatorVector();
        }
    }

    private void updateAnimatorVector()
    {
        _animator.SetFloat("AnimMoveX", _moveDirection.x);
        _animator.SetFloat("AnimMoveY", _moveDirection.y);
    }

    private void chargingSpell()
    {
        _timer += Time.deltaTime;
        _animator.SetFloat("HoldTime", _timer);
    }

    private void releaseSpell()
    {
        _timer += Time.deltaTime;
        
        if (_timer > spellCooldown)
        {
            _action = noAction;
            _move = mobile;
        }
    }

    private void noAction()
    {

    }

    private void updateFacing()
    {
        if ( _moveDirection.y < 0)
        {
            _facingDirection = Vector2.down;
        }
        else if ( _moveDirection.x < 0)
        {
            _facingDirection = Vector2.left;
        }
        else if ( _moveDirection.x > 0)
        {
            _facingDirection = Vector2.right;
        } 
        else
        {
            _facingDirection = Vector2.up;
        }
    }
}


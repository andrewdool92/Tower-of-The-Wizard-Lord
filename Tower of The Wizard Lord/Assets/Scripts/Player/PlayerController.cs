using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputReader _inputReader;
    [SerializeField] private float acceleration;
    [SerializeField] private float topSpeed;

    [SerializeField] private float spellCooldown;
    [SerializeField] private Spell defaultSpell;
    private Spell _spell;
    
    [SerializeField] ParticleSystem _spellParticles;
    [SerializeField] ParticleSystem _spellAura;
    private bool _particlesActive;
    private bool _auraActive;

    private Vector2 _moveDirection;
    private Vector2 _facingDirection;
    private Rigidbody2D _body;
    private Animator _animator;

    private delegate void Movement();
    Movement _move;

    private delegate void OnAction();
    OnAction _action;
    private float _timer;

    private ManaTracker _mana;


    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _move = mobile;
        _action = noAction;

        _inputReader = GameManager.Instance.inputReader;
        _inputReader.MoveEvent += handleMove;
        _inputReader.SpellcastEvent += handleSpellcast;
        _inputReader.SpellcastCancelledEvent += handleSpellcastCancelled;

        _mana = GameManager.Instance.playerMana;

        _spell = Instantiate(defaultSpell);
        _spellParticles = Instantiate(_spellParticles, this.transform);
        _spellAura = Instantiate(_spellAura, this.transform);

        _particlesActive = false;
        _auraActive = false;

        _facingDirection = Vector2.down;
    }

    private void OnDestroy()
    {
        _inputReader.MoveEvent -= handleMove;
        _inputReader.SpellcastEvent -= handleSpellcast;
        _inputReader.SpellcastCancelledEvent -= handleSpellcastCancelled;
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

        GameManager.Instance.updateMana(ManaPhase.casting);
    }

    void handleSpellcastCancelled()
    {
        _animator.SetBool("Casting", false);

        if (_timer < 0.08)
        {
            _move = mobile;
            _action = noAction;
        }
        else
        {
            _action = releaseSpell;
            _move = noAction;
            Vector2 recoil = _spell.activate(transform.position, _facingDirection, _timer);
            Debug.Log($"recoil: {recoil}");
            _body.AddForce(recoil);
        }
        GameManager.Instance.updateMana(ManaPhase.cancel);

        if (_particlesActive)
        {
            _spellParticles.Stop();
            _particlesActive = false;
        }
        if (_auraActive)
        {
            _spellAura.Stop();
            _auraActive = false;
        }
        _timer = 0;
    }

    void mobile()
    {
        if (_moveDirection != Vector2.zero)
        {
            _facingDirection = _moveDirection;
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
            _facingDirection = _moveDirection;
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

        if (_timer > 0.3 && !_auraActive)
        {
            _auraActive = true;
            _spellAura.Play();
        }
        if (_timer > 1 && !_particlesActive && _mana.Primed)
        {
            _particlesActive = true;
            _spellParticles.Play();
        }
        if (_spell.chargeThresholdReached(_timer))
        {
            GameManager.Instance.updateMana(ManaPhase.prime);
        }
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
}


using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] Animator _barrierAnimator;
    private bool _particlesActive;
    private bool _auraActive;

    private PitfallDropable _dropController;
    private Collider2D _collider;
    private float _fallTimer;

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

        _dropController = GetComponent<PitfallDropable>();
        _dropController.onPitfall += handlePitfall;
        _collider = GetComponent<Collider2D>();

        _spell = Instantiate(defaultSpell);
        _spellParticles = Instantiate(_spellParticles, this.transform);
        _spellAura = Instantiate(_spellAura, this.transform);

        _barrierAnimator = Instantiate(_barrierAnimator, this.transform);
        GameManager.PlayerDamageEvent += triggerBarrier;

        _particlesActive = false;
        _auraActive = false;

        _moveDirection = Vector2.up;
        rooted();                       // set the player's initial facing
        _moveDirection = Vector2.zero;
    }

    private void OnDestroy()
    {
        _inputReader.MoveEvent -= handleMove;
        _inputReader.SpellcastEvent -= handleSpellcast;
        _inputReader.SpellcastCancelledEvent -= handleSpellcastCancelled;
        _dropController.onPitfall -= handlePitfall;
        GameManager.PlayerDamageEvent -= triggerBarrier;
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
            _body.AddForce(recoil);
        }
        GameManager.Instance.updateMana(ManaPhase.cancel);

        stopPaticles();
        _timer = 0;
    }

    public void interuptSpellcast()
    {
        _animator.SetFloat("HoldTime", 0f);
        _animator.SetBool("Casting", false);
        _move = mobile;
        _action = noAction;

        stopPaticles();
    }

    private void stopPaticles()
    {
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

    public void disableControl()
    {
        interuptSpellcast();

        _move = noAction;
        _action = noAction;
    }

    public void resumeControl()
    {
        _move = mobile;
        _action = noAction;
    }

    private void handlePitfall(float delay)
    {
        _collider.enabled = false;
        _body.velocity = Vector3.zero;

        disableControl();
        _animator.Play("Base Layer.pitfall", 0, 0);
        _fallTimer = delay;
        _action = countdown;
    }

    private void countdown()
    {
        _fallTimer -= Time.deltaTime;
        if (_fallTimer < 0)
        {
            GameManager.Instance.updateMana(ManaPhase.damage);
            _collider.enabled = true;
            GameManager.Instance.updateFloor(-1);
            resumeControl();
        }
    }

    private void triggerBarrier()
    {
        if (_mana.Mana > 0)
        {
            _barrierAnimator.SetTrigger("block");
        }
        else if (_mana.Mana == 0)
        {
            _barrierAnimator.SetTrigger("break");
        }
    }
}


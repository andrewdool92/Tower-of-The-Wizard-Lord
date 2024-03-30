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
    [SerializeField] private Spell fireSpell;
    [SerializeField] private Spell iceSpell;
    private Dictionary<spellType, spellWrapper> _spells = new Dictionary<spellType, spellWrapper>();
    private spellWrapper _spell;
    
    private bool _particlesActive;
    private bool _auraActive;

    [SerializeField] Animator _barrierAnimator;
    [SerializeField] AudioClip[] _barrierBlockSound;
    [SerializeField] AudioClip[] _barrierBreakSound;
    [Range(0f, 1f)] public float barrierVolume = 1f;


    [SerializeField] AudioClip chargeFX;
    [SerializeField] float fadeinDuration;
    [Range(0f, 1f)] public float chargeVolume;
    AudioSource chargeSound;
    [SerializeField] float audioLoopPoint;  // the point at which the audio should loop back to the beginning
    [SerializeField] float audioJumpPoint;  // the point to jump to when the audio should stop

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

        setupSpells();
        equipSpell(_spells[spellType.starting]);

        _barrierAnimator = Instantiate(_barrierAnimator, this.transform);
        GameManager.PlayerDamageEvent += triggerBarrier;

        _particlesActive = false;
        _auraActive = false;

        chargeSound = AudioManager.Instance.createPersistentAudioSource(chargeFX, this.transform);

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

    public class spellWrapper
    {
        public Spell spell;
        ParticleSystem _chargeParticles;
        ParticleSystem _chargeAura;
        public spellWrapper(Spell spell, Transform parent)
        {
            this.spell = spell;
            _chargeParticles = Instantiate(this.spell.chargeParticles, parent);
            _chargeAura = Instantiate(this.spell.chargeAura, parent);
        }

        public void toggleParticles(bool active)
        {
            switch (active)
            {
                case true: _chargeParticles.Play(); break;
                case false: _chargeParticles.Stop(); break;
            }
        }

        public void toggleAura(bool active)
        {
            switch (active)
            {
                case true: _chargeAura.Play(); break;
                case false: _chargeAura.Stop(); break;
            }
        }

        public Vector2 activate(Vector2 position, Vector2 direction, float chargeTime)
        {
            return spell.activate(position, direction, chargeTime);
        }
    }

    private void setupSpells()
    {
        Debug.Log("setting up spells");
        Spell spell = Instantiate(defaultSpell, this.transform);
        _spells[spellType.starting] = new spellWrapper(Instantiate(defaultSpell, transform), this.transform);
        _spells[spellType.fire] = new spellWrapper(Instantiate(fireSpell), this.transform);
        _spells[spellType.ice] = new spellWrapper(Instantiate(iceSpell), this.transform);
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

    public void equipSpell(spellWrapper spell)
    {
        _spell = spell;
    }

    public void equipSpell(spellType spell)
    {
        _spell = _spells[spell];
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
        startChargeFX();
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

        completeChargeFX();
        stopPaticles();
        _timer = 0;
    }

    public void interuptSpellcast()
    {
        _animator.SetFloat("HoldTime", 0f);
        _animator.SetBool("Casting", false);
        _move = mobile;
        _action = noAction;

        cancelChargeFX();
        stopPaticles();
    }

    private void stopPaticles()
    {
        if (_particlesActive)
        {
            _spell.toggleParticles(false);
            _particlesActive = false;
        }
        if (_auraActive)
        {
            _spell.toggleAura(false);
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
        loopChargeFX();

        if (_timer > 0.3 && !_auraActive)
        {
            _auraActive = true;
            _spell.toggleAura(true);
        }
        if (_timer > 1 && !_particlesActive && _mana.Primed)
        {
            _particlesActive = true;
            _spell.toggleParticles(true);
        }
        if (_spell.spell.chargeThresholdReached(_timer))
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
            AudioManager.Instance.playRandomClip(_barrierBlockSound, transform, barrierVolume);
        }
        else if (_mana.Mana == 0)
        {
            AudioManager.Instance.playRandomClip(_barrierBreakSound, transform, barrierVolume);
            _barrierAnimator.SetTrigger("break");
        }
    }

    private void startChargeFX()
    {
        chargeSound = AudioManager.Instance.createPersistentAudioSource(chargeFX, transform);
        chargeSound.enabled = true;
        AudioManager.Instance.fadeInAudio(chargeSound, fadeinDuration, chargeVolume);
    }

    private void loopChargeFX()
    {
        if (chargeSound != null && chargeSound.time > audioLoopPoint)
        {
            chargeSound.time = 0;
        }
    }

    private void completeChargeFX()
    {
        if (chargeSound != null)
        {
            chargeSound.time = audioJumpPoint;
            Destroy(chargeSound.gameObject, 1f);
        }
    }

    private void cancelChargeFX()
    {
        if (chargeSound != null)
        {
            chargeSound.Stop();
            Destroy(chargeSound.gameObject);
        }
    }
}


public enum spellType
{
    starting,
    fire,
    ice
}

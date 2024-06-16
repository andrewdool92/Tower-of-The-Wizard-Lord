using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //[SerializeField] bool destroyOnImpact = false;     

    [SerializeField] float speedModifier;
    [SerializeField] float powerScaler;
    [SerializeField] float maxVelocity;
    [SerializeField] public float lifespan;
    [SerializeField] float[] impactRadius;
    [SerializeField] Vector2[] impactOffset;
    [SerializeField] public AudioClip[] impactFX;
    [SerializeField] float impactThreshold;

    protected Animator _animator;
    protected SpriteRenderer _sprite;
    protected Collider2D _projectileCollider;
    protected CircleCollider2D _impactCollider;
    protected Rigidbody2D _body;

    private int _power;

    protected float _timer;
    protected float _velocity;
    protected Vector3 _direction;

    protected delegate void OnUpdate();
    protected OnUpdate _onUpdate;

    protected delegate void OnCollision(UnityEngine.Collision2D collision);
    protected OnCollision _collision;

    // Start is called before the first frame update
    public virtual void Start()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _projectileCollider = GetComponent<Collider2D>();
        _impactCollider = gameObject.AddComponent<CircleCollider2D>();
        _body = GetComponent<Rigidbody2D>();
        _impactCollider.enabled = false;
        _impactCollider.radius = impactRadius[0];

        _sprite.enabled = false;
        _animator.enabled = false;
        _projectileCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        _onUpdate?.Invoke();
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        _collision?.Invoke(collision);
    }

    public virtual void _triggerImpact()
    {
        transform.up = Vector2.up;
        _velocity = 0;

        _animator.ResetTrigger("launch");
        _animator.SetTrigger("impact");
        _timer = 1;
        _projectileCollider.enabled = false;
        _impactCollider.enabled = true;

        if (impactFX[_power] != null)
        {
            AudioManager.Instance.playSoundClip(impactFX[_power], transform, 0.5f);
        }

        _onUpdate = _windDown;

        _collision = (collision) => { };
    }

    private void _windDown()
    {
        if (_timer < 0 )
        {
            _impactCollider.enabled = false;
            _projectileCollider.enabled = false;
            _sprite.enabled = false;
        }
    }

    private void _updateImpactRadius()
    {
        int index = (_velocity > impactThreshold) ? 1 : 0;
        _impactCollider.radius = impactRadius[index];
        _animator.SetInteger("size_index", index);
        _power = index;
    }


    public virtual void activeUpdate()
    {
        transform.position += _velocity * Time.deltaTime * _direction;
        if (_timer < 0 )
        {
            _triggerImpact();
        }
    }

    public virtual void activeCollision(UnityEngine.Collision2D other)
    {
        if (other.gameObject.CompareTag("blocker"))
        {
            _triggerImpact();
        }
    }

    public virtual void setActive(Vector3 direction)
    {
        _direction = direction;
        _onUpdate = activeUpdate;
        _collision = activeCollision;
    }

    public void launch(Vector2 position, Vector2 direction, float power)
    {
        transform.position = position;

        _projectileCollider.enabled = true;
        _sprite.enabled = true;
        _animator.enabled = true;

        _timer = lifespan;
        _velocity = Mathf.Clamp(power * Mathf.Pow(speedModifier, powerScaler), 0, maxVelocity);
        _updateImpactRadius();

        _animator.ResetTrigger("impact");
        _animator.SetTrigger("launch");

        setActive(new Vector3(direction.x, direction.y, 0));
    }

    public void setSprite(Sprite sprite)
    {
        _sprite.sprite = sprite;
    }
}

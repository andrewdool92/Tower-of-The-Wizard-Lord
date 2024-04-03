using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] bool isPhysicsObject = false;     //  determines logic for how this projectile is simulated
    [SerializeField] bool destroyOnImpact = false;     

    [SerializeField] float speedModifier;
    [SerializeField] float powerScaler;
    [SerializeField] float maxVelocity;
    [SerializeField] float lifespan;
    [SerializeField] float[] impactRadius;
    [SerializeField] Vector2[] impactOffset;
    [SerializeField] AudioClip[] impactFX;
    [SerializeField] float impactThreshold;

    private Animator _animator;
    private SpriteRenderer _sprite;
    private Collider2D _projectileCollider;
    private CircleCollider2D _impactCollider;
    private Rigidbody2D _body;

    private int _power;

    private float _timer;
    private float _velocity;

    private delegate void OnUpdate();
    private OnUpdate _onUpdate;

    private delegate void OnCollision(UnityEngine.Collision2D collision);
    private OnCollision _collision;

    // Start is called before the first frame update
    void Start()
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
        
        _timer = lifespan;
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        _onUpdate?.Invoke();
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Debug.Log("collision!");
        _collision?.Invoke(collision);
    }

    private void _triggerImpact()
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

        if (!isPhysicsObject)
        {
            transform.up = direction;
            _onUpdate = () =>
            {
                transform.position += transform.up * _velocity * Time.deltaTime;
                if (_timer < 0)
                {
                    _triggerImpact();
                }
            };

            _collision = (collision) =>
            {
                if (collision.gameObject.CompareTag("blocker"))
                {
                    _triggerImpact();
                }
            };
        }
        else
        {
            _body.velocity = direction.normalized * _velocity;
            _onUpdate = () =>
            {
                if (_timer < 0)
                {
                    _triggerImpact();
                }
            };

            _collision = (collision) =>
            {
                if (collision.gameObject.CompareTag("blocker"))
                {
                    _body.velocity = _body.velocity * -0.8f;
                }
            };
        }
    }
}

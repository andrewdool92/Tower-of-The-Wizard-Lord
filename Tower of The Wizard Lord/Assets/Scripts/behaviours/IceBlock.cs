using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class IceBlock : Projectile
{
    [SerializeField] float damagePerCollision;
    [SerializeField] float collisionDamageThreshold;

    private damageable _dmg;
    private ParticleSystem _particles;

    public override void Start()
    {
        base.Start();

        _particles = GetComponent<ParticleSystem>();

        _dmg = gameObject.AddComponent<damageable>();
        _dmg.damageEvent += takeDamage;
        _dmg.immunityUpdateEvent += setMobile;

        _animator.enabled = false;
    }

    private void OnDestroy()
    {
        _dmg.damageEvent -= takeDamage;
        _dmg.immunityUpdateEvent -= setMobile;
    }


    public override void setActive(Vector3 direction)
    {
        base.setActive(direction);
        _animator.enabled = true;
        _body.velocity = _direction.normalized * _velocity;
    }

    public override void activeUpdate()
    {
        if (_timer < 0)
        {
            _projectileCollider.enabled = false;
            _animator.enabled = false;
            _sprite.enabled = false;

            _onUpdate = null;
            _collision = null;
        }
        else
        {
            _animator.SetFloat("Size", _timer / lifespan);
        }
    }

    public override void activeCollision(Collision2D other)
    {
        if (other.gameObject.CompareTag("blocker"))
        {
            _body.velocity *= -0.8f;
            print($"relative velocity: {other.relativeVelocity.magnitude}");
            if (other.relativeVelocity.magnitude > collisionDamageThreshold)
            {
                takeDamage(1);
            }
        }
    }

    private void takeDamage(int damage)
    {
        _timer -= damagePerCollision * damage;
        _particles.Play();
        AudioManager.Instance.playRandomClip(impactFX, transform, 0.5f);
    }

    private void setMobile(bool locked)
    {
        if (locked)
        {
            _body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}

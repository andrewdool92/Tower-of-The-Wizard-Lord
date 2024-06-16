using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageable : MonoBehaviour
{
    [SerializeField] float immunityTime = 0;
    private float _timer;

    private Rigidbody2D _rb;

    private delegate void OnUpdate(float dt);
    OnUpdate onUpdate;

    public bool immune = false;
    public event Action<int> damageEvent;
    public event Action<bool> immunityUpdateEvent;
    public event Action<float, Vector2, float> knockbackEvent;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();

        onUpdate = doNothing;
    }

    private void Update()
    {
        onUpdate(Time.deltaTime);
    }

    public void takeDamage(int damage)
    {
        takeDamage(damage, 0, Vector2.zero, 0);
    }

    private void applyKnockback(float force, Vector2 direction, float duration)
    {
        if (knockbackEvent != null && duration > 0)
        {
            knockbackEvent.Invoke(force, direction, duration);
        }
        else if (direction != Vector2.zero)
        {
            _rb.velocity = direction;
        }
    }

    public void takeDamage(int damage, float knockForce, Vector2 knockDirection, float duration)
    {
        if (!immune)
        {
            damageEvent?.Invoke(damage);
            applyKnockback(knockForce, knockDirection, duration);

            immune = true;
            immunityUpdateEvent?.Invoke(immune);

            _timer = immunityTime;
            onUpdate = immunityTimer;
        }
    }

    public void takeDamage(int damage, Vector2 force, float duration)
    {
        takeDamage(damage, 1, force, duration);
    }

    public void takeDamage(int damage, float force, float duration)
    {
        takeDamage(damage, force, Vector2.zero, duration);
    }

    private void doNothing(float dt) { }

    private void immunityTimer(float dt)
    {
        _timer -= dt;
        if (_timer < 0 )
        {
            immune = false;
            immunityUpdateEvent?.Invoke(immune);

            onUpdate = doNothing;
        }
    }

}

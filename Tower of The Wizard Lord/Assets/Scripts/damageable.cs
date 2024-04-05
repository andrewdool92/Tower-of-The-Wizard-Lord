using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageable : MonoBehaviour
{
    [SerializeField] bool player;
    [SerializeField] float immunityTime;
    private float _timer;

    private ManaTracker _mana;
    private Rigidbody2D _rb;
    private PlayerController _playerController;

    private delegate void OnUpdate(float dt);
    OnUpdate onUpdate;

    public bool immune = false;

    // Start is called before the first frame update
    void Start()
    {
        if (player)
        {
            _mana = GameManager.Instance.playerMana;
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _playerController = gameObject.GetComponent<PlayerController>();
        }

        onUpdate = doNothing;
    }

    private void Update()
    {
        onUpdate(Time.deltaTime);
    }

    public void takeDamage(int damage)
    {
        if (player && !immune)
        {
            Debug.Log("Damage instance registered");
            _playerController.updateBarrier(true);
            for (int i = 0; i < damage; i++)
            {
                GameManager.Instance.updateMana(ManaPhase.damage);
            }
            immune = true;
            _timer = immunityTime;
            onUpdate = immunityTimer;
        }
    }

    public void takeDamageWithKnockback(int damage, Vector2 force)
    {
        takeDamage(damage);
        _rb?.AddForce(force);
    }

    private void doNothing(float dt) { }

    private void immunityTimer(float dt)
    {
        _timer -= dt;
        if (_timer < 0 )
        {
            immune = false;
            onUpdate = doNothing;
            _playerController.updateBarrier(false);
        }
    }

}

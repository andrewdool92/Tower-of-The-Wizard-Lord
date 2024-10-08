using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField] float activeTime;
    [SerializeField] bool cardinalDirectionLock;
    [SerializeField] int objectPoolSize = 1;
    private int _poolIndex = 0;

    [SerializeField] Vector2[] colliderSizes;
    [SerializeField] float[] offsets;
    [SerializeField] float chargeSpeed;

    [SerializeField] public ParticleSystem chargeParticles;
    [SerializeField] public ParticleSystem chargeAura;

    [SerializeField] Projectile projectile;
    [SerializeField] int projectileChargeThreshold;
    [SerializeField] float recoilModifier;
    private Projectile[] _projectiles;
    float timer;

    private Transform _transform;
    private BoxCollider2D[] _colliders;
    private Collider2D _activeCollider;
    private Animator _animator;

    [SerializeField] AudioClip[] audioFX;

    private delegate void update();
    private update onUpdate;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _setupColliders();

        timer = 0;
        initializeProjectilePool();

        onUpdate = noActivity;
    }

    private void Update()
    {
        onUpdate();
    }

    private void initializeProjectilePool()
    {
        _projectiles = new Projectile[objectPoolSize];

        for (int i = 0; i < objectPoolSize; i++)
        {
            _projectiles[i] = Instantiate(projectile);
        }
    }

    private Projectile getProjectileFromPool()
    {
        _poolIndex = (_poolIndex + 1) % objectPoolSize;
        return _projectiles[_poolIndex];
    }

    private void _setupColliders()
    {
        _colliders = new BoxCollider2D[colliderSizes.Length];
        
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i] = gameObject.AddComponent<BoxCollider2D>();
            _colliders[i].size = colliderSizes[i];
            _colliders[i].enabled = false;
        }

        _activeCollider = _colliders[0];
    }

    void noActivity() { }

    public float getChargePower(float chargeTime)
    {
        return chargeTime * chargeSpeed;
    }

    public bool chargeThresholdReached(float chargeTime)
    {
        return getChargePower(chargeTime) > projectileChargeThreshold;
    }


    public Vector2 activate(Vector2 position, Vector2 direction, float chargeTime)
    {
        projectile = getProjectileFromPool();

        float power = chargeTime * chargeSpeed;

        if (!GameManager.Instance.playerMana.Primed)     // limit spell power when mana is not available
        {
            power = Mathf.Clamp(power, 0, projectileChargeThreshold);
            GameManager.Instance.playerMana.Primed = false;
        }

        int powerIndex = (int)Mathf.Clamp(Mathf.Floor(power), 0, offsets.Length - 1);   // Fit power index to available objects
        direction = cardinalDirectionLock ? toCardinal(direction) : direction;

        _transform.up = direction;
        _transform.position = position + direction * offsets[powerIndex];
        _activeCollider = _colliders[powerIndex];

        timer = activeTime;
        _activeCollider.enabled = true;
        _animator.SetFloat ("Power", power);
        _animator.SetTrigger("Activate");

        if (audioFX[powerIndex] != null)
        {
            AudioManager.Instance.playSoundClip(audioFX[powerIndex], transform, 0.5f);
        }

        if (power > projectileChargeThreshold && projectile != null)
        {
            projectile.launch(transform.position, direction, power);
        }

        onUpdate = advance;

        return (power > projectileChargeThreshold) ? direction * -1 * power * recoilModifier : Vector2.zero;
        // returns the recoil that can be applied to the caster
    }

    void advance()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            _activeCollider.enabled = false;

            onUpdate = noActivity;
        }
    }

    private Vector2 toCardinal(Vector2 direction)
    {
        float absY = Mathf.Abs(direction.y);
        float absX = Mathf.Abs(direction.x);

        if (direction.y < 0 && absY >= absX) { return Vector2.down; }
        else if (absX >= absY)
        {
            return direction.x < 0 ? Vector2.left : Vector2.right;
        }
        return Vector2.up;

    }
}

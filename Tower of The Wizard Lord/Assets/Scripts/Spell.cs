using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField] float activeTime;
    [SerializeField] bool cardinalDirectionLock;
    [SerializeField] Vector2[] colliderSizes;
    [SerializeField] float[] offsets;
    [SerializeField] float chargeSpeed;
    [SerializeField] Projectile projectile;
    [SerializeField] int projectileChargeThreshold;
    [SerializeField] float recoilModifier;
    float timer;

    private Transform _transform;
    private BoxCollider2D[] _colliders;
    private Collider2D _activeCollider;
    private Animator _animator;

    [SerializeField] AudioClip[] audioFX;

    private delegate void update();
    private update onUpdate;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _setupColliders();
        projectile = Instantiate(projectile);

        timer = 0;

        onUpdate = noActivity;
    }

    private void Update()
    {
        onUpdate();
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


    public Vector2 activate(Vector2 position, Vector2 direction, float chargeTime)
    {
        float power = chargeTime * chargeSpeed;
        int powerIndex = (int)Mathf.Clamp(Mathf.Floor(power), 0, offsets.Length - 1);   // limits index by what spell powers actually exist
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

        //Debug.Log($"Casting spell with power {power}");

        if (power >= projectileChargeThreshold && projectile != null)
        {
            projectile.launch(transform.position, direction, power);
        }

        onUpdate = advance;

        return (power > 2) ? direction * -1 * power * recoilModifier : Vector2.zero;
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

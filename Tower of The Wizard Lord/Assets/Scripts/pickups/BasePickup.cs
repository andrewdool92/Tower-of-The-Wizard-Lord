using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : MonoBehaviour
{
    [SerializeField] public bool Respawn = true;
    [SerializeField] public float ReformationTime;
    [SerializeField] public AudioClip[] pickupSFX;

    protected Collider2D _collider;
    public virtual Collider2D Collider { get { return _collider; } set { _collider = value; } }

    protected Animator _animator;
    protected float _timer;

    protected delegate void OnUpdate();
    protected OnUpdate onUpdate;


    // Start is called before the first frame update
    public virtual void Start()
    {
        _timer = 0;

        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponentInChildren<Collider2D>();

        onUpdate = doNothing;
    }

    // Update is called once per frame
    void Update()
    {
        onUpdate();
    }

    public void basePickup()
    {
        AudioManager.Instance.playRandomClip(pickupSFX, transform, 0.5f);
        toggleCollider(false);

        if (_animator != null)
        {
            _animator.SetTrigger("pickup");
        }
        if (Respawn)
        {
            setTimer();
            onUpdate = reform;
        }
    }

    public virtual void reform()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _animator.SetTrigger("reform");
            _collider.enabled = true;
            onUpdate = doNothing;
        }

    }

    public void setTimer()
    {
        _timer = ReformationTime;
    }

    public void doNothing() { }

    public virtual void toggleCollider(bool state)
    {
        if (Collider != null)
        {
            Collider.enabled = state;
        }
    }
}

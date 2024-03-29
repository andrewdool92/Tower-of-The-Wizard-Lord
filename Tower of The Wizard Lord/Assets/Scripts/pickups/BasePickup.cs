using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : MonoBehaviour
{
    [SerializeField] public float ReformationTime;
    [SerializeField] public AudioClip[] pickupSFX;

    protected Collider2D _collider;
    protected Animator _animator;
    protected float _timer;

    protected delegate void OnUpdate();
    protected OnUpdate onUpdate;


    // Start is called before the first frame update
    public virtual void Start()
    {
        _timer = 0;

        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();

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
        _collider.enabled = false;
    }

    public void doNothing() { }
}

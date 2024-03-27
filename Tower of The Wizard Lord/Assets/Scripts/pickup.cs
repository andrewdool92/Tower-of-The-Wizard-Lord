using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] float ReformationTime;
    [SerializeField] AudioClip[] pickupSFX;

    private Collider2D _collider;
    private Animator _animator;
    private float _timer;

    private delegate void OnUpdate();
    private OnUpdate onUpdate;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;

        _animator = GetComponentInChildren<Animator>();
        _animator.Play("Base Layer.idle", 0, Random.value);

        _collider = GetComponent<Collider2D>();

        onUpdate = doNothing;
    }

    // Update is called once per frame
    void Update()
    {
        onUpdate();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.instance.updateMana(ManaPhase.pickup);
        AudioManager.Instance.playRandomClip(pickupSFX, transform, 0.5f);

        _collider.enabled = false;
        _animator.SetTrigger("pickup");
        _timer = ReformationTime;
        onUpdate = reform;
    }

    void reform()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _animator.SetTrigger("reform");
            _collider.enabled = true;
            onUpdate = doNothing;
        }
    }

    void doNothing()
    {
        //Debug.Log("doing nothing");
    }
}

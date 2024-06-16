using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : BasePickup
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockBack;
    [SerializeField] float rootTime;
    [SerializeField] float warningTime;

    private bool _resetting = false;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

    }


    void OnTriggerEnter2D(Collider2D other)
    {
        basePickup();

        if (other.transform.TryGetComponent<damageable>(out var dmg))
        {
            dmg.takeDamage(damage, knockBack, rootTime);
        }

        _timer = ReformationTime;
        _resetting = true;
        onUpdate = reform;
    }

    public override void reform()
    {
        base.reform();
        
        if (_resetting && _timer < warningTime)
        {
            _animator.SetTrigger("startReset");
            _resetting = false;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManaPickup : BasePickup
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _animator.Play("Base Layer.idle", 0, Random.value);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        basePickup();

        GameManager.Instance.updateMana(ManaPhase.pickup);
        _animator.SetTrigger("pickup");
        _timer = ReformationTime;
        onUpdate = reform;
    }

}

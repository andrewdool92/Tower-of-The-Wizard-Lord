using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPickup : BasePickup
{
    [SerializeField] spellType spell;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        basePickup();

        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.equipSpell(spell);
        }
    }
}

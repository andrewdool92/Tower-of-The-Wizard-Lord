using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageable : MonoBehaviour
{
    [SerializeField] bool player;
    private ManaTracker _mana;

    // Start is called before the first frame update
    void Start()
    {
        if (player)
        {
            _mana = GameManager.Instance.playerMana;
        }
    }

    public void takeDamage(int damage)
    {
        if (player)
        {
            for (int i = 0; i < damage; i++)
            {
                GameManager.Instance.updateMana(ManaPhase.damage);
            }
        }
    }

}

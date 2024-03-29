using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManaBar : MonoBehaviour
{
    [SerializeField] GameObject manaPip;
    [SerializeField] float offset;

    private Animator[] manaPipAnimators;
    private PipState[] manaPipStates;
    private ManaTracker _playerMana;

    // Start is called before the first frame update
    void Start()
    {
        _playerMana = GameManager.Instance.playerMana;
        int maxMana = _playerMana.MaxMana;
        int startingMana = _playerMana.Mana;

        manaPipAnimators = new Animator[maxMana];
        manaPipStates = new PipState[maxMana];

        for (int i = 0; i < maxMana; i++)
        {
            GameObject pip = Instantiate(manaPip, transform);
            pip.name = $"pip ({i})";

            pip.GetComponent<RectTransform>().anchoredPosition = new Vector3(-offset * (maxMana - i), 0, 0);
            manaPipAnimators[i] = pip.GetComponentInChildren<Animator>();

            if (i <= startingMana - 1)
            {
                manaPipAnimators[i].Play("Base Layer.charged", 0, Random.value);
                manaPipStates[i] = PipState.full;
            }
            else
            {
                manaPipAnimators[i].Play("Base Layer.empty", 0, Random.value);
                manaPipStates[i] = PipState.empty;
            }
        }

        GameManager.ManaUpdateEvent += handleManaUpdate;
    }

    private void OnDestroy()
    {
        GameManager.ManaUpdateEvent -= handleManaUpdate;
    }

    private void handleManaUpdate(ManaPhase phase)
    {
        switch (phase)
        {
            case ManaPhase.casting:
                startCasting();
                break;
            case ManaPhase.prime:
                primeMana();
                break;
            case ManaPhase.cancel:
                cancelManaChange();
                break;
            case ManaPhase.charging:
                startCharging();
                break;
            case ManaPhase.full:
                fillPip();
                break;
            case ManaPhase.pickup:
                startCharging();
                fillPip();
                break;
            case ManaPhase.damage:
                takeDamage();
                break;
        }
    }

    private void startCasting()
    {
        if (_playerMana.Mana > 0)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetBool("casting", true);
            pip.SetBool("charging", false);
        }
    }

    private void primeMana()
    {
        if (_playerMana.Mana > 0 && !_playerMana.Primed)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetTrigger("prime");
            pip.ResetTrigger("completeCharge");
            _playerMana.Mana -= 1;
        }
    }

    private void cancelManaChange()
    {
        foreach (Animator animator in manaPipAnimators)
        {
            animator.SetBool("casting", false);
            animator.SetBool("charging", false);
        }
        _playerMana.Primed = false;
    }

    private void startCharging()
    {
        if (_playerMana.Mana < _playerMana.MaxMana)
        {
            manaPipAnimators[_playerMana.Mana].SetBool("charging", true);
        }
    }

    private void fillPip()
    {
        if (_playerMana.Mana < _playerMana.MaxMana)
        {
            manaPipAnimators[_playerMana.Mana].SetTrigger("completeCharge");
            _playerMana.Mana += 1;
        }
    }

    private void takeDamage()
    {
        if (_playerMana.Mana > 0)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetTrigger("damage");
            pip.SetBool("charging", false);
        }
        _playerMana.Mana -= 1;
    }
}

public enum PipState
{
    full,
    primed,
    empty,
    charging
}

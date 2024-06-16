using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManaBar : MonoBehaviour
{
    [SerializeField] GameObject manaPip;
    [SerializeField] float offset;
    [SerializeField] AudioClip[] primeSound;
    [Range(0f, 1f)] public float primeVolume = 1f;

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
            case ManaPhase.cast:
                endCasting();
                break;
        }
        Debug.Log($"Mana: {_playerMana.Mana}; state: {phase}");
    }

    private void startCasting()
    {
        if (_playerMana.Mana > 0)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetBool("interrupt", false);
            pip.SetBool("casting", true);
            pip.SetBool("charging", false);
            pip.ResetTrigger("cancel");
        }
    }

    private void endCasting()
    {
        Animator pip = manaPipAnimators[_playerMana.Mana - 1];

        if (_playerMana.Primed && _playerMana.Mana > 0)
        {
            _playerMana.Primed = false;
            _playerMana.Mana -= 1;
        }
        else
        {
            pip.SetBool("interrupt", true);
            pip.SetTrigger("cancel");
        }
        pip.SetBool("casting", false);
        pip.SetBool("charging", false);
    }

    private void primeMana()
    {
        if (_playerMana.Mana > 0 && !_playerMana.Primed)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetBool("interrupt", false);
            pip.SetTrigger("prime");
            pip.ResetTrigger("completeCharge");
            _playerMana.Primed = true;

            AudioManager.Instance.playRandomClip(primeSound, transform, primeVolume);
        }
    }

    private void cancelManaChange()
    {
        Animator pip = manaPipAnimators[_playerMana.Mana - 1];
        pip.SetBool("casting", false);
        pip.SetBool("charging", false);
        pip.SetTrigger("cancel");
    }

    private void startCharging()
    {
        if (_playerMana.Mana < _playerMana.MaxMana)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana];
            pip.ResetTrigger("cancel");
            pip.SetBool("interrupt", false);
            pip.SetBool("charging", true);
        }
    }

    private void fillPip()
    {
        if (_playerMana.Primed && _playerMana.Mana < _playerMana.MaxMana)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetBool("interrupt", true);
            pip.SetBool("casting", false);
            pip.SetTrigger("cancel");

            _playerMana.Mana += 1;
            pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetBool("casting", true);
            pip.SetTrigger("prime");
        }
        else if (_playerMana.Mana < _playerMana.MaxMana)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana];
            pip.SetBool("interrupt", true);
            pip.SetTrigger("completeCharge");
            _playerMana.Mana += 1;

        }
    }

    private void takeDamage()
    {
        if (_playerMana.Mana > 0)
        {
            Animator pip = manaPipAnimators[_playerMana.Mana - 1];
            pip.SetBool("interrupt", true);

            pip.SetTrigger("damage");
            pip.SetBool("casting", false);
            pip.SetBool("charging", false);
            _playerMana.Primed = false;
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

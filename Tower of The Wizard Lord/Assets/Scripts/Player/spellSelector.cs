using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class spellSelector : MonoBehaviour
{
    private Dictionary<spellType, bool> enabledSpells = new Dictionary<spellType, bool>();
    private PlayerController playerController;

    private InputReader _inputReader;
    private Vector2 _lastInput;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        
        enabledSpells[spellType.fire] = false;
        enabledSpells[spellType.ice] = false;

        _inputReader = GameManager.Instance.inputReader;
        _inputReader.SpellSelectEvent += handleSpellSelectInput;
    }

    private void OnDestroy()
    {
        _inputReader.SpellSelectEvent -= handleSpellSelectInput;
    }

    void handleSpellSelectInput(Vector2 input)
    {
        if (input != Vector2.zero)
        {
            _lastInput = toCardinal(input);
        }
        else if (_lastInput != Vector2.zero)
        {
            equipSpellFromInput(_lastInput);
            _lastInput = Vector2.zero;
        }
    }

    public void enableSpell(spellType spell)
    {
        enabledSpells[spell] = true;

        // temporary logic ahead:
        if (spell == spellType.fire)
        {
            GameManager.Instance.updateSpell(spell);
            DialogueEventManager.Instance.displayTutorial(Tutorial.manaSpend);
        }

        if (spell == spellType.ice)
        {
            DialogueEventManager.Instance.displayTutorial(Tutorial.swap);
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

    private void equipSpellFromInput(Vector2 input)
    {
        spellType spell = spellType.starting;

        if (input == Vector2.up && enabledSpells[spellType.fire])
        {
            spell = spellType.fire;
        }
        else if (input == Vector2.down && enabledSpells[spellType.ice])
        {
            spell = spellType.ice;
        }

        if (spell != spellType.starting)
        {
            GameManager.Instance.updateSpell(spell);
        }
    }
}

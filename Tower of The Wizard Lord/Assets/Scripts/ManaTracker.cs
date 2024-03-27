using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaTracker
{
    private int _mana;
    private int _maxMana;

    public int Mana
    {
        get
        {
            return _mana;
        }
        set
        {
            _mana = value;
        }
    }

    public int MaxMana
    {
        get
        {
            return _maxMana;
        }
        set
        {
            _maxMana = value;
        }
    }

    public bool Primed { get; set; }

    public ManaTracker(int startingMana, int maxMana)
    {
        _mana = startingMana;
        _maxMana = maxMana;
    }
}

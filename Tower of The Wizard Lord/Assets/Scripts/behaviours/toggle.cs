using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggle : MonoBehaviour
{
    [SerializeField] bool autoToggle;
    [SerializeField] float flipBackTime;
    [SerializeField] bool invertOnState;
    [SerializeField] bool startState = false;

    public virtual event Action<bool, int> OnFlip;

    public bool _switchState { get; set; }

    public virtual void flip(bool state)
    {
        state = invertOnState ? !state : state;
        OnFlip?.Invoke(state, this.GetInstanceID());
    }

    private void Start()
    {
        _switchState = startState;
    }
}

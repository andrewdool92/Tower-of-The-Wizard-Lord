using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSwitch
{
    public virtual event Action<bool> OnFlip;

    public abstract bool GetState();

    public virtual void activateTrigger(bool state)
    {
        OnFlip?.Invoke(state);
    }
}

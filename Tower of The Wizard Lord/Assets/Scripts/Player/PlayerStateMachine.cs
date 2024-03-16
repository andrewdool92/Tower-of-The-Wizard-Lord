using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState;

    public void changeState(PlayerState newState)
    {
        currentState = newState;
    }
}

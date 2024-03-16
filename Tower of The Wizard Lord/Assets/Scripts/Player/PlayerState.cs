using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerState
{
    private PlayerStateMachine _stateMachine;
    private Animator _animator;
    private Transform _transform;

    private float startTime;

    public PlayerState(PlayerStateMachine stateMachine, Animator animator, Transform transform)
    {
        _stateMachine = stateMachine;
        _animator = animator;
        _transform = transform;
    }

    public virtual void enter()
    {
        startTime = Time.time;
    }

    public virtual void exit()
    {

    }

    public virtual void move(Vector2 direction)
    {

    }
}

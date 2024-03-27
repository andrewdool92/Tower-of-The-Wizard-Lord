/**
 * Script to handle inputs and create input events
 * New entities that use inputs will require access to the inputReader through a serialized field,
 *  then they can subscribe their event handlers to the relevant events. Example code below.
 */

/**
 * [SerializeField] private InputReader inputReader;
 * 
 * void Start()
 * {
 *      inputReader.ClickEvent += handleClick;
 * }
 * 
 * void onDestroy()
 * {
 *      inputReader.ClickEvent -= handleClick;
 * }
 * 
 * private void handleClick()
 * {
 *      Debug.log("clicked!");
 * }
 */

using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IUIActions
{
    private GameInput _gameInput;

    public event Action<Vector2> MoveEvent;

    public event Action SpellcastEvent;
    public event Action SpellcastCancelledEvent;

    public event Action ClickEvent;
    public event Action ClickCancelledEvent;

    public event Action PauseEvent;
    public event Action ResumeEvent;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.gameplay.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);
        }
    }

    public void setGameplay()
    {
        _gameInput.gameplay.Enable();
        _gameInput.UI.Disable();
    }

    public void setUI()
    {
        _gameInput.UI.Enable();
        _gameInput.gameplay.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
            setUI();
        }
    }

    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ResumeEvent?.Invoke();
            setGameplay();
        }
    }

    public void OnSpellcast(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SpellcastEvent?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            SpellcastCancelledEvent?.Invoke();
        }
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ClickEvent?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            ClickCancelledEvent?.Invoke();
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}


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
public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IUIActions, GameInput.IDialogueActions
{
    private GameInput _gameInput;

    public event Action<Vector2> MoveEvent;

    public event Action SpellcastEvent;
    public event Action SpellcastCancelledEvent;

    public event Action<Vector2> SpellSelectEvent;

    public event Action ClickEvent;
    public event Action ClickCancelledEvent;

    public event Action PauseEvent;
    public event Action ResumeEvent;

    public event Action continueDialogueEvent;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.gameplay.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);
            _gameInput.dialogue.SetCallbacks(this);
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

    public void startDialogue()
    {
        _gameInput.dialogue.Enable();
        _gameInput.gameplay.Disable();
    }

    public void endDialogue()
    {
        _gameInput.gameplay.Enable();
        _gameInput.dialogue.Disable();
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

    }

    public void OnSubmit(InputAction.CallbackContext context)
    {

    }

    public void OnCancel(InputAction.CallbackContext context)
    {

    }

    public void OnPoint(InputAction.CallbackContext context)
    {

    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {

    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {

    }

    public void OnRightClick(InputAction.CallbackContext context)
    {

    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {

    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        
    }

    public void OnContinue(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            continueDialogueEvent?.Invoke();
        }
    }

    public void OnSpellSelect(InputAction.CallbackContext context)
    {
        SpellSelectEvent?.Invoke(context.ReadValue<Vector2>());
    }
}


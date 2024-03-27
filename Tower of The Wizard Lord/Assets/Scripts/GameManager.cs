using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject().AddComponent<GameManager>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    public GameState state;

    public InputReader inputReader { get; private set; }
    private GameObject ui;

    public static event Action<GameState> OnGameStateChanged;

    public ManaTracker playerMana = new ManaTracker(5, 5);
    public static event Action<ManaPhase> ManaUpdateEvent;

    void Awake()
    {
        inputReader = ScriptableObject.CreateInstance<InputReader>();
    }

    private void Start()
    {
        inputReader.PauseEvent += handlePauseInput;
        inputReader.ResumeEvent += handleResumeInput;

        inputReader.setGameplay();
    }

    public void updateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.gameplay:
                handleResume();
                break;
            case GameState.pause:
                handlePause();
                break;
            case GameState.restart:
                handleRestart();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void handlePauseInput()
    {
        updateGameState(GameState.pause);
    }

    private void handleResumeInput()
    {
        updateGameState(GameState.gameplay);
    }

    private void handlePause()
    {
        inputReader.setUI();
        Time.timeScale = 0f;
    }

    private void handleResume()
    {
        inputReader.setGameplay();
        Time.timeScale = 1;
    }

    private void handleRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void updateMana(ManaPhase phase)
    {
        switch (phase)
        {
            case ManaPhase.prime:
                if (playerMana.Mana > 0)
                {
                    ManaUpdateEvent?.Invoke(phase);
                    playerMana.Primed = true;
                }
                break;
            case ManaPhase.charging:
                if (playerMana.Mana < playerMana.MaxMana)
                {
                    ManaUpdateEvent?.Invoke(phase);
                }
                break;
            default:
                ManaUpdateEvent?.Invoke(phase);
                break;
        }
    }

}

public enum GameState
{
    main,
    gameplay,
    pause,
    restart,
}

public enum ManaPhase
{
    casting,
    prime,
    charging,
    full,
    cancel
}

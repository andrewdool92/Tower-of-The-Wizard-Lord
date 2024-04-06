using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager Instance
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
    public static event Action<gameOver> OnGameOver;

    public ManaTracker playerMana = new ManaTracker(5, 5);
    public static event Action<ManaPhase> ManaUpdateEvent;
    public static event Action PlayerDamageEvent;

    public static event Action<int> FloorUpdateEvent;

    public static event Action StartDialogueEvent;
    public static event Action ContinueDialogueEvent;
    public static event Action ExitDialogueEvent;

    public static event Action<spellType> spellSelectEvent;

    void Awake()
    {
        inputReader = ScriptableObject.CreateInstance<InputReader>();
    }

    private void Start()
    {
        inputReader.PauseEvent += handlePauseInput;
        inputReader.ResumeEvent += handleResumeInput;
        inputReader.continueDialogueEvent += handleContinueDialogue;

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
            case GameState.main:
                handleMainMenu();
                break;
            case GameState.tutorial:
                handleTutorial();
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
        playerMana.reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void handleMainMenu()
    {
        inputReader.setUI();
        Time.timeScale = 1;
        SceneManager.LoadScene("SplashScreen");
    }

    private void handleTutorial()
    {
        playerMana.reset();
        SceneManager.LoadScene("IntroTutorial");
        inputReader.setGameplay();
    }

    public void triggerVictory()
    {
        inputReader.setUI();
        OnGameOver?.Invoke(gameOver.win);
    }

    private void checkGameOver()
    {
        if (playerMana.Mana < 0)
        {
            inputReader.setUI();
            OnGameOver?.Invoke(gameOver.lose);
        }
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
            case ManaPhase.damage:
                ManaUpdateEvent?.Invoke(phase);
                PlayerDamageEvent?.Invoke();        // needed to separate to deal with race condition on player barrier
                break;
            default:
                ManaUpdateEvent?.Invoke(phase);
                break;
        }
        checkGameOver();
    }

    public void updateFloor(int direction)
    {
        GameManager.FloorUpdateEvent?.Invoke(direction);
    }

    public void enterDialogue()
    {
        inputReader.startDialogue();
        Time.timeScale = 0;
        StartDialogueEvent?.Invoke();
    }

    public void endDialogue()
    {
        inputReader.endDialogue();
        Time.timeScale = 1;
        ExitDialogueEvent?.Invoke();
    }

    private void handleContinueDialogue()
    {
        ContinueDialogueEvent?.Invoke();
    }

    public void updateSpell(spellType spell)
    {
        if (spell != spellType.starting)
        {
            spellSelectEvent?.Invoke(spell);
        }
    }
}

public enum GameState
{
    main,
    gameplay,
    pause,
    restart,
    tutorial
}

public enum ManaPhase
{
    casting,
    prime,
    charging,
    full,
    cancel,
    pickup,
    damage
}

public enum gameOver
{
    win,
    lose
}


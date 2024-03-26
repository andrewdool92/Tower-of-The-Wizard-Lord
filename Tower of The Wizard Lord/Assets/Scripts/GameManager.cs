using System;
using System.Collections;
using System.Collections.Generic;
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
}

public enum GameState
{
    main,
    gameplay,
    pause,
    restart,
}

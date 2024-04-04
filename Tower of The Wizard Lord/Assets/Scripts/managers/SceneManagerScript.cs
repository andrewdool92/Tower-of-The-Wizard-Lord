using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] AudioClip backgroundMusic;
    [Range(0f, 1f)] public float volume;
    private AudioSource music;

    private void Awake()
    {
        music = AudioManager.Instance.createPersistentAudioSource(backgroundMusic, transform);
        music.spatialBlend = 0;
        music.volume = volume;
        music.loop = true;
        music.Play();

        GameManager.OnGameOver += fadeMusic;
        GameManager.OnGameStateChanged += handleGameState;
    }

    private void handleGameState(GameState state)
    {
        if (state == GameState.gameplay)
        {
            music.Play();
        }
        else if (state == GameState.pause)
        {
            music.Stop();
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= fadeMusic;
        GameManager.OnGameStateChanged -= handleGameState;
    }

    private void fadeMusic()
    {
        AudioManager.Instance.fadeAudio(music, 1);
    }



    public void LoadScene(string scene)
    {
        GameState newState;


        if (scene == "IntroTutorial")
        {
            newState = GameState.tutorial;
        }
        else if (scene == "SplashScreen")
        {
            newState = GameState.main;
        }
        else if (scene == "restart")
        {
            newState = GameState.restart;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }

        GameManager.Instance.updateGameState(newState);
    }
}

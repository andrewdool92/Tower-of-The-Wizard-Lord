using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        GameState newState;

        Debug.Log($"Called LoadScene with string: {scene}");

        if (scene == "IntroTutorial")
        {
            Debug.Log("It did get here");
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

        Debug.Log($"Next state: {newState}");

        GameManager.Instance.updateGameState(newState);
    }
}

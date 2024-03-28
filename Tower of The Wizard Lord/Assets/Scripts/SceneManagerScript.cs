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

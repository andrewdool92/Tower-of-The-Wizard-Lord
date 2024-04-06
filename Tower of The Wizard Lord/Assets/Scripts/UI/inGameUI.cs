using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class inGameUI : MonoBehaviour
{
    [SerializeField] bool activateOnStart;
    [SerializeField] bool activateOnGameOver;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.gameObject.SetActive(activateOnStart);

        GameManager.OnGameOver += handleGameOver;
    }

    protected virtual void OnDestroy()
    {
        GameManager.OnGameOver -= handleGameOver;
    }

    protected virtual void handleGameOver(gameOver state)
    {
        gameObject.SetActive(activateOnGameOver);
    }


}

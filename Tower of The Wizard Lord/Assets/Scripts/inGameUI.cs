using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inGameUI : MonoBehaviour
{
    [SerializeField] bool activateOnStart;
    [SerializeField] bool activateOnGameOver;


    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(activateOnStart);

        GameManager.OnGameOver += () => this.gameObject.SetActive(activateOnGameOver);
    }

}

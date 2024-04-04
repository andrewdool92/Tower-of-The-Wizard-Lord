using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inGameUI : MonoBehaviour
{
    [SerializeField] bool activateOnStart;
    [SerializeField] bool activateOnGameOver;

    [SerializeField] AudioClip gameOverSound;
    [Range(0f, 1f)] public float gameOverVolume;


    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(activateOnStart);

        GameManager.OnGameOver += handleGameOver;
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= handleGameOver;
    }

    private void handleGameOver()
    {
        gameObject.SetActive(activateOnGameOver);
        if (gameOverSound != null)
        {
            AudioManager.Instance.playSoundClip(gameOverSound, transform, gameOverVolume);
        }
    }

}

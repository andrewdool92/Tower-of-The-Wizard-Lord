using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScreen : inGameUI
{
    [SerializeField] AudioClip gameOverSound;
    [Range(0f, 1f)] public float gameOverVolume;

    [SerializeField] fireworks victoryConfetti;
    [SerializeField] AudioClip vicotrySound;
    public float confettiOffset;
    public float confettiTimeOffset;
    [Range(0f, 1f)] public float vicotryVolume;

    private fireworks _confetti1;
    private fireworks _confetti2;

    public string gameOverMessage;
    public string victoryMessage;
    private TextMeshProUGUI label;


    protected override void Start()
    {
        base.Start();
        instantiateConfetti();
        label = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void instantiateConfetti()
    {
        if (victoryConfetti != null)
        {
            Transform player = FindObjectOfType<PlayerController>().transform;

            _confetti1 = Instantiate(victoryConfetti, player);
            _confetti1.transform.position += Vector3.left * confettiOffset;

            _confetti2 = Instantiate(victoryConfetti, player);
            _confetti2.transform.position += Vector3.right * confettiOffset;
            _confetti2.startDelay = confettiTimeOffset;
        }
    }

    private void activateConfetti()
    {
        _confetti1.startTimer();
        _confetti2.startTimer();
    }

    protected override void handleGameOver(gameOver state)
    {
        switch (state)
        {
            case gameOver.lose:
                AudioManager.Instance.playSoundClip(gameOverSound, transform, gameOverVolume);
                label.text = gameOverMessage;
                break;
            case gameOver.win:
                AudioManager.Instance.playSoundClip(vicotrySound, transform, vicotryVolume);
                label.text = victoryMessage;
                activateConfetti();
                break;
        }

        gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOverTrigger : MonoBehaviour
{
    [SerializeField] gameOver condition;


    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.triggerVictory();
        gameObject.SetActive(false);
    }
}

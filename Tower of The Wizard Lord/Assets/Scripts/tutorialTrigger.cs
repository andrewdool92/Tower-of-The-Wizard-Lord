using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialTrigger : MonoBehaviour
{
    [SerializeField] Tutorial tutorial;


    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DialogueEventManager.Instance.displayTutorial(tutorial);
        gameObject.SetActive(false);
    }
}

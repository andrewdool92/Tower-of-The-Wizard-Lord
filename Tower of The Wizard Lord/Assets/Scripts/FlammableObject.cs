using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlammableObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        Debug.Log($"Entered trigger with tag: {collision.gameObject.tag}");

        if ( collision.gameObject.CompareTag("fire"))
        {
            GetComponent<Animator>().SetBool("on_fire", true);
        }
    }
}

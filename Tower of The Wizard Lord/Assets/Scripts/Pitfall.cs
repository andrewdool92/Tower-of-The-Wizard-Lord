using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : MonoBehaviour
{

    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.TryGetComponent<PitfallDropable>(out  PitfallDropable entity))
        {
            entity.drop();
        }
    }
}

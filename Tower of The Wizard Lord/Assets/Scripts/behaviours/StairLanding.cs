using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairLanding : MonoBehaviour
{
    [SerializeField] float floorOffset;
    [SerializeField] Floor landingLevel;

    private int _direction;     // Indicates the change in floor resulting from climbing the stairs in this direction
    
    // Start is called before the first frame update
    void Start()
    {
        _direction = (landingLevel == Floor.lower) ? 1 : -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.position += Vector3.up * floorOffset * _direction;

        GameManager.Instance.updateFloor(_direction);
    }
}

public enum Floor
{
    upper,
    lower
}

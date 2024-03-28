using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class floorTracker : MonoBehaviour
{
    private TextMeshProUGUI label;
    private int _currentFloor;

    // Start is called before the first frame update
    void Start()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
        _currentFloor = 1;

        GameManager.FloorUpdateEvent += handleFloorChange;
    }

    void handleFloorChange(int change)
    {
        _currentFloor += change;
        label.text = $"FLOOR: {_currentFloor}";
    }

}

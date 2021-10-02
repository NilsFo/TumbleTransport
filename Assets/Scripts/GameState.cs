using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum SelectionState: UInt16
    {
        None,
        Cargo,
        Tool
    }

    public GameObject debugTruck;
    public SelectionState currentSelectionState;
    
    // Start is called before the first frame update
    void Start()
    {
        print("Game has been started! Let's gooooooo!");
        currentSelectionState = SelectionState.None;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool HasSomethingSelected()
    {
        return currentSelectionState != SelectionState.None;
    }

    public GameObject GetCurrentTruck()
    {
        Debug.LogWarning("DEBUG TRUCK NEEDS TO BE SWAPPED WITH REAL TRUCK");
        return debugTruck;
    }
    
    
}

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
        
    public float workTime = 12f;
    public float workTimeLeft = 0;
    public float workHoursPerMinute = 1.0f;

    public SelectionState currentSelectionState;
    
    public TruckSpawner truckSpawner;
    public SpawnCargo cargoSpawner;

    public Score score;
    
    // Start is called before the first frame update
    void Start()
    {
        currentSelectionState = SelectionState.None;
        truckSpawner.CurrentSpawnerState = TruckSpawner.SpawnerState.On;
        workTimeLeft = workTime;
        
        score = new Score();
    }

    void Update() {
        workTimeLeft -= Time.deltaTime * workHoursPerMinute / 60;
    }

    public bool HasSomethingSelected()
    {
        return currentSelectionState != SelectionState.None;
    }

    public GameObject GetCurrentTruck()
    {
        return truckSpawner.TruckGameObject;
    }
}

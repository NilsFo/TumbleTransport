using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static int shift = 0;
    public static Score score = new Score();

    public enum SelectionState : UInt16
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
    public ToolConveyor toolConveyor;

    // Start is called before the first frame update
    void Start()
    {
        currentSelectionState = SelectionState.None;
        truckSpawner.CurrentSpawnerState = TruckSpawner.SpawnerState.On;
        workTimeLeft = workTime;

        score = new Score();
    }

    void Update()
    {
        workTimeLeft -= Time.deltaTime * workHoursPerMinute / 60;
        if (workTimeLeft < 0)
        {
            SceneManager.LoadScene("EndScene");
        }
    }

    public void SubtractMaterialCost(int cost)
    {
        //TODO: Remove cost from pay
        Debug.LogWarning("TODO: COST NEEDS TO BE DEDUCTED FROM SCORE: " + cost);
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
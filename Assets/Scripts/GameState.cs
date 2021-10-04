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
    public bool worktimeDecayEnabled = true;

    public SelectionState currentSelectionState;

    public TruckSpawner truckSpawner;
    public SpawnCargo cargoSpawner;
    public ToolConveyor toolConveyor;

    public bool tutorialRunning = false;
    public bool forceNextDriverQuote = false;
    public bool tutorialHasLoadedTruckAtLeastOnce = true;
    public bool tutorialHasDepartedAtLeastOnce = true;
    public bool tutorialHasTooledAtLeastOnce = true;

    public bool tutorialHasRopedAtLeastOnce = true;
    public bool tutorialHasGluedAtLeastOnce = true;
    public bool tutorialHasTapedAtLeastOnce = true;

    // Start is called before the first frame update
    void Start()
    {
        currentSelectionState = SelectionState.None;
        truckSpawner.CurrentSpawnerState = TruckSpawner.SpawnerState.On;
        workTimeLeft = workTime;
        score = new Score();

        bool tutorialLevel = true;
        // TODO if not the first level, clear tutorial flags!
        if (tutorialLevel)
        {
            worktimeDecayEnabled = false;
            tutorialHasLoadedTruckAtLeastOnce = false;
            tutorialHasDepartedAtLeastOnce = false;
            tutorialHasTooledAtLeastOnce = false;

            tutorialHasRopedAtLeastOnce = false;
            tutorialHasGluedAtLeastOnce = false;
            tutorialHasTapedAtLeastOnce = false;
            tutorialRunning = true;

            toolConveyor.maxSpawnedItems = 0;
            toolConveyor.pendingSpawnCount = 0;
        }
        else
        {
            cargoSpawner.maxNumberOfCargo = 3;
        }
    }

    void Update()
    {
        if (worktimeDecayEnabled)
        {
            workTimeLeft -= Time.deltaTime * workHoursPerMinute / 60;
            if (workTimeLeft < 0)
            {
                SceneManager.LoadScene("EndScene");
            }
        }
    }

    public void SubtractMaterialCost(float cost, string toolname, Sprite Image)
    {
        score.AddWorkingMaterialData(new WorkingMaterialData(cost, toolname), Image);
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
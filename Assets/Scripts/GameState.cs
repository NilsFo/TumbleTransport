using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static int shift = -1; //Init mit -1 für newbie
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
    public StartButtonTruck startBT;
    
    public bool tutorialHasTapeDroppedAtLeastOnce = false;

    public bool tutorialRunning = false;
    public bool forceNextDriverQuote = false;
    public bool tutorialHasLoadedTruckAtLeastOnce = true;
    public bool tutorialHasDepartedAtLeastOnce = true;
    public bool tutorialHasTooledAtLeastOnce = true;
    public bool tutorialHasDeletedAtLeastOnce = true;
    public bool tutorialHasRotatedAtLeastOnce = true;

    public bool tutorialHasRopedAtLeastOnce = true;
    public bool tutorialHasGluedAtLeastOnce = true;
    public bool tutorialHasTapedAtLeastOnce = true;

    public bool shiftEnded;
    public AudioSource shiftEndedSound;
    public AudioSource boxPickupSound;
    public AudioSource boxDropSound;

    public Transform nightMode;
    
    public static float lastProfit = 0; //Profit
    public static int lastShift = -1; //lastShift
    
    public static float bestProfitShift0 = 0; //Profit Shift 0
    public static float bestProfitShift1 = 0; //Profit Shift 1
    public static float bestProfitShift2 = 0; //Profit Shift 2

    // Start is called before the first frame update
    void Start()
    {
        currentSelectionState = SelectionState.None;
        truckSpawner.CurrentSpawnerState = TruckSpawner.SpawnerState.On;
        workTimeLeft = workTime;
        score = new Score();

        bool tutorialLevel = GameState.shift == 0;
        if (tutorialLevel)
        {
            worktimeDecayEnabled = false;
            tutorialHasLoadedTruckAtLeastOnce = false;
            tutorialHasDepartedAtLeastOnce = false;
            tutorialHasTooledAtLeastOnce = false;
            tutorialHasDeletedAtLeastOnce = false;
            tutorialHasRotatedAtLeastOnce = false;

            tutorialHasRopedAtLeastOnce = false;
            tutorialHasGluedAtLeastOnce = false;
            tutorialHasTapedAtLeastOnce = false;
            tutorialRunning = true;

            toolConveyor.maxSpawnedItems = 0;
            toolConveyor.pendingSpawnCount = 0;
            toolConveyor.tutorialToolOverrideState = 2;

            startBT.myText.enabled = false;
        } else {
            toolConveyor.maxSpawnedItems = 3;
        }
        tutorialHasTapeDroppedAtLeastOnce = false;

        if (GameState.shift == 0)
        {
            cargoSpawner.maxNumberOfCargo = 8;
        } 
        else if (GameState.shift == 1)
        {
            cargoSpawner.maxNumberOfCargo = 10;
        }
        else if (GameState.shift == 2)
        {
            cargoSpawner.maxNumberOfCargo = 12;
            nightMode.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (worktimeDecayEnabled) {
            workTimeLeft -= Time.deltaTime * workHoursPerMinute / 60f;
            if (workTimeLeft < 0) {
                shiftEnded = true;
                worktimeDecayEnabled = false;
                shiftEndedSound.Play();
                FindObjectOfType<RadioManager>().RadioMessage("Shift's over! Finish up, there's no pay for overtime!", 10);
            }
        }
        
        
        // ESC quits to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key was pressed");
            SceneManager.LoadScene("Scenes/MenuScene");
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

    public static void loadDaat()
    {
        if (PlayerPrefs.HasKey("lastProfit"))
        {
            lastProfit = PlayerPrefs.GetFloat("lastProfit");
        }
        if (PlayerPrefs.HasKey("lastShift"))
        {
            lastShift = PlayerPrefs.GetInt("lastShift");
        }
        if (PlayerPrefs.HasKey("bestProfitShift0"))
        {
            bestProfitShift0 = PlayerPrefs.GetFloat("bestProfitShift0");
        }
        if (PlayerPrefs.HasKey("bestProfitShift1"))
        {
            bestProfitShift1 = PlayerPrefs.GetFloat("bestProfitShift1");
        }
        if (PlayerPrefs.HasKey("bestProfitShift2"))
        {
            bestProfitShift2 = PlayerPrefs.GetFloat("bestProfitShift2");
        }

        /*
        Debug.Log(lastProfit);
        Debug.Log(lastShift);
        Debug.Log(bestProfitShift0);
        Debug.Log(bestProfitShift1);
        Debug.Log(bestProfitShift2);
        Debug.Log("Load!!!"); 
        */
    }

    public static void saveData()
    {
        PlayerPrefs.SetFloat("lastProfit", lastProfit);
        PlayerPrefs.SetInt("lastShift", lastShift);
        PlayerPrefs.SetFloat("bestProfitShift0", bestProfitShift0);
        PlayerPrefs.SetFloat("bestProfitShift1", bestProfitShift1);
        PlayerPrefs.SetFloat("bestProfitShift2", bestProfitShift2);
        PlayerPrefs.Save();
        /*
        Debug.Log(lastProfit);
        Debug.Log(lastShift);
        Debug.Log(bestProfitShift0);
        Debug.Log(bestProfitShift1);
        Debug.Log(bestProfitShift2);
        Debug.Log("Save!!!"); 
        */
    }
}
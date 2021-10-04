using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TruckSpawner : MonoBehaviour
{
    public GameState gameState;
    
    public TruckSpawnPoolScriptableObject[] spawnPools;

    public Transform spawnPoint;

    public float spawnTimerDelay = 5f;
    
    private float _spawnTimer = 0f;

    private GameObject _truckGameObject;
    private Truck _truck;

    public enum SpawnerState
    {
        On, Off
    }

    private SpawnerState _currentSpawnerState = SpawnerState.Off; 

    public SpawnerState CurrentSpawnerState
    {
        get => _currentSpawnerState;
        set => _currentSpawnerState = value;
    }

    public GameObject TruckGameObject
    {
        get => _truckGameObject;
    }

    private void Update()
    {
        if (_truckGameObject != null)
        {
            if (_truck.State == Truck.TruckState.Traveling) {
                var data = new DeliveryData(_truck.truckData, _truck.fastenedCargo, _truck.thrownCargo);
                GameState.score.AddDeliveryData(data);
                _truck.CreateFloatingText((int)(data.GetSalesProfit()));
                Destroy(_truckGameObject);
                _truckGameObject = null;
                _truck = null;
                
                if (gameState.shiftEnded) {
                    StartCoroutine(LoadEndSceneAsync());
                }
            }
        }
        else
        {
            if (_spawnTimer < spawnTimerDelay && _currentSpawnerState == SpawnerState.On)
            {
                _spawnTimer += Time.deltaTime;
                if (_spawnTimer <= spawnTimerDelay)
                {
                    SpawnTruck();
                }
            }
        }
    }

    IEnumerator LoadEndSceneAsync() {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("EndScene", LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(GameObject.Find("/Audio"), SceneManager.GetSceneByName("EndScene"));
        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }

    public void DispatchTruck()
    {
        if (_truck != null)
        {
            _truck.Dispatch();
        }
    }

    private GameObject SpawnTruck()
    {
        if(!gameState.tutorialRunning)
            FindObjectOfType<RadioManager>().ReadRandomQuote();

        TruckSpawnPoolScriptableObject spawnPool = spawnPools[GameState.shift];
        
        int pickWeight = Random.Range(0 ,spawnPool.totalWeight);

        TruckScriptableObject myTruckData = spawnPool.pool[0];
        GameObject spawnPrefab = myTruckData.truck;
        pickWeight -= myTruckData.weight;
        for (int i = 1; i < spawnPool.pool.Length; i++)
        {
            TruckScriptableObject myTruck = spawnPool.pool[i];
            if (pickWeight > 0)
            {
                myTruckData = myTruck;
                pickWeight -= myTruck.weight;
            }
            else
            {
                break;
            }
        }

        GameObject myTruckGameObject = Instantiate(myTruckData.truck, spawnPoint.position, Quaternion.identity);
        Truck myTruckComp = myTruckGameObject.GetComponent<Truck>();
        if (myTruckComp != null)
        {
            myTruckComp.truckData = myTruckData;
            _truck = myTruckComp;
        }
        _truckGameObject = myTruckGameObject;

        gameState.cargoSpawner.SpawnFixNumberOfCargo(myTruckData.fixNumberOfCargoToSpawn);
        gameState.toolConveyor.AddPendingSpawns(4);
        if (!gameState.toolConveyor.HasRope()) {
            gameState.toolConveyor.ropeGuaranteed = true;
        }
        
        return myTruckGameObject;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TruckSpawner : MonoBehaviour
{
    public GameState gameState;
    
    public TruckSpawnPoolScriptableObject spawnPool;
    
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
            if (_truck.State == Truck.TruckState.Traveling)
            {
                
                GameState.score.AddDeliveryData(new DeliveryData(_truck.truckData, _truck.fastenedCargo, _truck.thrownCargo));
                Destroy(_truckGameObject);
                _truckGameObject = null;
                _truck = null;
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

    public void DispatchTruck()
    {
        if (_truck != null)
        {
            _truck.Dispatch();
        }
    }

    private GameObject SpawnTruck()
    {
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
        
        return myTruckGameObject;
    }
}

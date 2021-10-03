using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TruckSpawner : MonoBehaviour
{
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
                    _truckGameObject = SpawnTruck();
                    _truck = _truckGameObject.GetComponent<Truck>();
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
        Debug.Log("Spawning Truck");
        FindObjectOfType<RadioManager>().ReadRandomQuote();
        
        int pickWeight = Random.Range(0 ,spawnPool.totalWeight);

        TruckScriptableObject init = spawnPool.pool[0];
        GameObject spawnPrefab = init.truck;
        pickWeight -= init.weight;
        for (int i = 1; i < spawnPool.pool.Length; i++)
        {
            TruckScriptableObject myCargo = spawnPool.pool[i];
            if (pickWeight > 0)
            {
                spawnPrefab = myCargo.truck;
                pickWeight -= myCargo.weight;
            }
            else
            {
                break;
            }
        }

        GameObject myTruckGameObject = Instantiate(spawnPrefab, spawnPoint.position, Quaternion.identity);
        return myTruckGameObject;
    }
}

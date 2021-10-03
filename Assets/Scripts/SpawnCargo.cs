using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class SpawnCargo : MonoBehaviour
{

    public SpawnPoolScriptableObject spawnPool;

    public BoxCollider2D spawnArea;
    public Transform spawnPoint;

    public float minAirTime = 1.5f;
    public float maxAirTime = 2f;
    
    public float burstDelay = 0.4f;

    public int maxNumberOfCargo = 10;

    public float spawnTime = 5f;
    private float _currentTime = 0f;

    // Update is called once per frame
    void Update() {
        // TODO: ACHTUNG! Nach genug Zeit wird der Spawner hinter die Kamera rücken.
        // Dafür sorgen dass die Spawnerposition nach einem Arbeitstag zurückgesetzt wird 
        // Dies sorgt dafür dass die Collider nicht alle auf der selben Z-Ebene sind, was zu wonky physics führt
        spawnPoint.position -= Time.deltaTime * new Vector3(0, 0, 0.01f);
        
        if (_currentTime <= spawnTime)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            TrySpawnCargo();
            _currentTime = 0f;
        }
    }

    public void SpawnFixNumberOfCargo(int numberToSpawn)
    {
        reminderToSpawn = numberToSpawn;
        Invoke(nameof(_SpawnFixNumberOfCargo), burstDelay);
    }

    private int reminderToSpawn = 0;
    private void  _SpawnFixNumberOfCargo()
    {
        TrySpawnCargo();
        reminderToSpawn -= 1;
        if (reminderToSpawn > 0)
        {
            Invoke(nameof(_SpawnFixNumberOfCargo), burstDelay);
        }
    }

    void TrySpawnCargo()
    {
        Cargo[] CargoList = FindObjectsOfType<Cargo>();
        Dictionary<string, bool> uniqueCargo =
            new Dictionary<string, bool>();

        for (int i = 0; i < CargoList.Length; i++)
        {
            Cargo myCargo = CargoList[i];
            if (myCargo.dataObject && myCargo.dataObject.isUnique)
            {
                uniqueCargo.Add(myCargo.dataObject.sprite.name, true);
            }
        }
            
        if (CargoList.Length < maxNumberOfCargo)
        {
            SpawnCargoInArea(uniqueCargo);
        }
    }
    
    void SpawnCargoInArea(Dictionary<string, bool> uniqueCargo = null)
    {
        int pickWeight = Random.Range(0 ,spawnPool.totalWeight);

        CargoScriptableObject myCargoData = spawnPool.pool[0];
        pickWeight -= myCargoData.weight;
        for (int i = 1; i < spawnPool.pool.Length; i++)
        {
            CargoScriptableObject pick = spawnPool.pool[i];
            if (pickWeight > 0 && uniqueCargo != null)
            {
                if (!uniqueCargo.ContainsKey(pick.sprite.name))
                {
                    myCargoData = pick;
                    pickWeight -= pick.weight;
                }
            }
            else
            {
                break;
            }
        }
        
        GameObject spawnPrefab = myCargoData.sprite;

        SpriteRenderer mySpriteRenderer = spawnPrefab.GetComponent<SpriteRenderer>();

        var bounds = spawnArea.bounds;
        var spriteExtents = mySpriteRenderer.bounds.extents;
        Vector2 spriteArea = new Vector2(spriteExtents.x, spriteExtents.y);

        float xpos = 0;
        float ypos = 0;
            
        xpos = Random.Range((bounds.min.x + spriteExtents.x) , (bounds.max.x - spriteExtents.x));
        ypos = Random.Range((bounds.min.y + spriteExtents.y) , (bounds.max.y - spriteExtents.y));
            
        var endPos = new Vector3(xpos, ypos, 0);
            
        GameObject myCargoGameObject = Instantiate(spawnPrefab, spawnPoint.position, Quaternion.identity);
        CargoSpawnAnimator myAnmimator = myCargoGameObject.GetComponent<CargoSpawnAnimator>();
        if (myAnmimator != null)
        {
            myAnmimator.startPos = spawnPoint.position;
            myAnmimator.targetPos = endPos;
            myAnmimator.maxAirTime = Random.Range(minAirTime, maxAirTime);
            myAnmimator.bounceNumber = Random.Range(2, 5);
        }
        
        Cargo myCargo = myCargoGameObject.GetComponent<Cargo>();
        if (myCargo != null)
        {
            myCargo.dataObject = myCargoData;
        }
    }
}

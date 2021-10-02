using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnCargo : MonoBehaviour
{

    public SpawnPoolScriptableObject spawnPool;

    public BoxCollider2D spawnArea;
    public Transform spawnPoint;

    public float minAirTime = 1.5f;
    public float maxAirTime = 2f;

    public int maxNumberOfCargo = 20;

    public float spawnTime = 2f;
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
            GameObject[] Cargo = GameObject.FindGameObjectsWithTag("Cargo");
            if (Cargo.Length < maxNumberOfCargo)
            {
                SpawnCargoInArea();
            }
            _currentTime = 0f;
        }
    }

    void SpawnCargoInArea()
    {
        int pickWeight = Random.Range(0 ,spawnPool.totalWeight);

        CargoScriptableObject init = spawnPool.pool[0];
        GameObject spawnPrefab = init.sprite;
        pickWeight -= init.weight;
        for (int i = 1; i < spawnPool.pool.Length; i++)
        {
            CargoScriptableObject myCargo = spawnPool.pool[i];
            if (pickWeight > 0)
            {
                spawnPrefab = myCargo.sprite;
                pickWeight -= myCargo.weight;
            }
            else
            {
                break;
            }
        }

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
    }
}

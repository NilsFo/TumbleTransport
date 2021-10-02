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

    public  ContactFilter2D filter;
    
    public float minAirTime = 2f;
    public float maxAirTime = 5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            int pickWeight = Random.Range(0 ,spawnPool.totalWeight);

            var init = spawnPool.pool[0];
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnCargo : MonoBehaviour
{

    public SpawnPoolScriptableObject spawnPool;

    public BoxCollider2D spawnArea;

    public  ContactFilter2D filter;
    
    public int maxRetrysForPlacement = 20;

    private void Start()
    {
        
    }

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
            var spriteExtents = mySpriteRenderer.bounds.size;
            Vector2 spriteArea = new Vector2(spriteExtents.x, spriteExtents.y);

            float xpos = 0;
            float ypos = 0;
            int numberOfTrys = 0;
            int spaceIsOccupied = 0;
            
            do
            {
                xpos = Random.Range((bounds.min.x + spriteExtents.x) , (bounds.max.x - spriteExtents.x));
                ypos = Random.Range((bounds.min.y + spriteExtents.y) , (bounds.max.y - spriteExtents.y));

                numberOfTrys++;
                Collider2D[] resulte = new Collider2D[1];
                spaceIsOccupied = Physics2D.OverlapBox(new Vector2(xpos, ypos), spriteArea, 0f, filter, resulte);
            } while ((numberOfTrys < maxRetrysForPlacement) && spaceIsOccupied == 0);

            Instantiate(spawnPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity);
        }
    }
}

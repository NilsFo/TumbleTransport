using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using UnityEngine;

public class ToolConveyor : MonoBehaviour
{
    private List<GameObject> myTools;

    private float spawnTimerCurrent;
    public float spawnTimer = 5;
    public float speed = 2;

    public int maxSpawnItems = 5;
    public List<GameObject> spwawnables;

    public float toolPositionOffset = .5f;

    public float maxDistance = 3.5f;
    public float creationXPos = 10;

    public GameObject debugSpawn;

    public List<Sprite> animationKeyFrames;

    private void Start()
    {
        myTools = new List<GameObject>();
    }

    private void Update()
    {
        // Update Tool Position
        bool shouldMove = false;
        for (int i = 0; i < GetToolCount(); i++)
        {
            GameObject tool = myTools[i];
            Vector3 pos = tool.transform.localPosition;
            float maxToolDist = maxDistance - toolPositionOffset * i;
            float newy = pos.y + speed*Time.deltaTime;
 
            if (newy >= maxToolDist){
                newy = maxToolDist;
            }else{
                shouldMove = true;
            }

            tool.transform.localPosition = new Vector3(pos.x,  newy, pos.z);
        }

        // SpawnNext
        if (GetToolCount() < maxSpawnItems)
        {
            spawnTimerCurrent = spawnTimerCurrent + Time.deltaTime;
            if (spawnTimerCurrent > spawnTimer)
            {
                SpawnNext();
            }
        }
        else
        {
            spawnTimerCurrent = 0;
        }

        if (Input.GetKeyDown("e"))
        {
            Remove(UnityEngine.Random.Range(1,GetToolCount())-1);
        }
    }

    public void Remove(int index)
    {
        print("Removing index "+index);
        GameObject toolToRemove = myTools[index];
        myTools.RemoveAt(index);
        Destroy(toolToRemove);
    }

    private void SpawnNext()
    {
        print("spawning next item");
        GameObject newTool = Instantiate(debugSpawn,gameObject.transform);
        myTools.Add(newTool);
        newTool.transform.localPosition = new Vector3(creationXPos, 0, transform.localPosition.z-1);

        spawnTimerCurrent = 0;
    }


    public int GetToolCount()
    {
        return myTools.Count;
    }
}
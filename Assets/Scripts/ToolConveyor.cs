using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolConveyor : MonoBehaviour
{
    private GameState gameState;
    private List<GameObject> myTools;
    public GameObject toolIndicator;

    private float spawnTimerCurrent;
    public float spawnTimer = 5;
    public float speed = 2;

    public int maxSpawnItems = 5;
    public List<GameObject> spwawnables;

    public float toolPositionOffset = .5f;

    public float maxDistance = 3.5f;
    public float creationXPos = 10;

    public GameObject debugSpawn;
    public GameObject putAwayArea;
    public GameObject dumpsterArea;

    public SpriteRenderer mySprite;
    public float animationSpeed = 1;
    private float animationSpeedCurrent = 0;
    public int currentKeyFrame = 0;
    public List<Sprite> animationKeyFrames;

    private void Start()
    {
        myTools = new List<GameObject>();
        animationSpeedCurrent = 0;
        currentKeyFrame = 0;
        mySprite.sprite = animationKeyFrames[currentKeyFrame];
        gameState = FindObjectOfType<GameState>();
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

        // Debug Remove
        if (Input.GetKeyDown("e"))
        {
            Remove(UnityEngine.Random.Range(1,GetToolCount())-1);
        }
        
        // Updating animation keyframe
        if (shouldMove && gameState.currentSelectionState != GameState.SelectionState.Tool)
        {
            animationSpeedCurrent = animationSpeedCurrent + Time.deltaTime;
            if (animationSpeedCurrent > animationSpeed)
            {
                animationSpeedCurrent -= animationSpeed;
                currentKeyFrame = (currentKeyFrame + 1) % animationKeyFrames.Count;
                mySprite.sprite = animationKeyFrames[currentKeyFrame];
            }
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
        newTool.GetComponent<ToolPickupButton>().conveyorCallback = this;
        newTool.GetComponent<ToolPickupButton>().conveyorIndex = myTools.Count-1;
        newTool.GetComponent<ToolPickupButton>().putawayArea = putAwayArea.GetComponent<Collider2D>();
        newTool.GetComponent<ToolPickupButton>().dumpsterArea = dumpsterArea.GetComponent<Collider2D>();
        
        ToolPickupButton buttonAI = newTool.GetComponent<ToolPickupButton>();
        buttonAI.toolUsageIndicator = toolIndicator;

        spawnTimerCurrent = 0;
    }

    public int GetToolCount()
    {
        return myTools.Count;
    }
    
    
}
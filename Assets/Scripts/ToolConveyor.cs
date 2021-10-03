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

    public int maxSpawnedItems = 4;
    public List<GameObject> spwawnables;

    public float toolPositionOffset = .5f;

    public float maxDistance = 3.5f;
    public float creationOffsetX = 0;
    public float creationOffsetY = -2;

    public GameObject putAwayArea;
    public GameObject dumpsterArea;

    public SpriteRenderer mySpriteLower;
    public SpriteRenderer mySpriteUpper;
    public float animationSpeed = 1;
    private float animationSpeedCurrent = 0;
    public int currentKeyFrame = 0;
    public List<Sprite> animationKeyFrames;

    public int pendingSpawnCount = 4;
    public bool ropeGuaranteed = true;
    public GameObject ropePrefab;

    private void Start()
    {
        toolIndicator.gameObject.SetActive(false);
        myTools = new List<GameObject>();
        animationSpeedCurrent = 0;
        currentKeyFrame = 0;
        mySpriteLower.sprite = animationKeyFrames[currentKeyFrame];
        mySpriteUpper.sprite = animationKeyFrames[currentKeyFrame];
        gameState = FindObjectOfType<GameState>();
        pendingSpawnCount = maxSpawnedItems;
    }

    private void Update()
    {
        // Update Tool Position
        bool shouldAnimate = false;
        if (gameState.currentSelectionState != GameState.SelectionState.Tool)
        {
            for (int i = 0; i < GetToolCount(); i++)
            {
                GameObject tool = myTools[i];
                Vector3 pos = tool.transform.localPosition;
                float maxToolDist = maxDistance - toolPositionOffset * i;
                float newy = pos.y + speed * Time.deltaTime;

                if (newy >= maxToolDist)
                {
                    newy = maxToolDist;
                }
                else
                {
                    shouldAnimate = true;
                }

                tool.transform.localPosition = new Vector3(pos.x, newy, pos.z);
            }
        }

        // SpawnNext
        if (GetToolCount() < maxSpawnedItems)
        {
            spawnTimerCurrent = spawnTimerCurrent + Time.deltaTime;
            if (spawnTimerCurrent > spawnTimer && pendingSpawnCount > 0)
            {
                SpawnNext();
            }
        }
        else
        {
            spawnTimerCurrent = 0;
        }

        // Updating animation keyframe
        if (shouldAnimate && gameState.currentSelectionState != GameState.SelectionState.Tool)
        {
            animationSpeedCurrent = animationSpeedCurrent + Time.deltaTime;
            if (animationSpeedCurrent > animationSpeed)
            {
                animationSpeedCurrent -= animationSpeed;
                currentKeyFrame = (currentKeyFrame + 1) % animationKeyFrames.Count;
                mySpriteLower.sprite = animationKeyFrames[currentKeyFrame];
                mySpriteUpper.sprite = animationKeyFrames[currentKeyFrame];
            }
        }
    }

    public void Remove(GameObject obj)
    {
        if (myTools.Contains(obj))
        {
            myTools.Remove(obj);
            Destroy(obj);
        }
        else
        {
            Debug.LogWarning("Warning! Request to delete illegal tool!!! " + gameObject);
        }
    }

    private void SpawnNext()
    {
        GameObject newTool = Instantiate(SelectNextTool(), gameObject.transform);
        myTools.Add(newTool);
        newTool.transform.localPosition = new Vector3(creationOffsetX, creationOffsetY, transform.localPosition.z - 1);

        ToolPickupButtonRope buttonRopeAI = newTool.GetComponent<ToolPickupButtonRope>();
        ToolPickupButtonGlue buttonGlueAI = newTool.GetComponent<ToolPickupButtonGlue>();
        ToolPickupButtonTape buttonTapeAI = newTool.GetComponent<ToolPickupButtonTape>();
        if (buttonTapeAI != null)
        {
            buttonTapeAI.toolUsageIndicator = toolIndicator;
            buttonTapeAI.conveyorCallback = this;
            buttonTapeAI.putawayArea = putAwayArea.GetComponent<Collider2D>();
            buttonTapeAI.dumpsterArea = dumpsterArea.GetComponent<Collider2D>();
        }

        if (buttonGlueAI != null)
        {
            buttonGlueAI.toolUsageIndicator = toolIndicator;
            buttonGlueAI.toolUsageIndicator = toolIndicator;
            buttonGlueAI.conveyorCallback = this;
            buttonGlueAI.putawayArea = putAwayArea.GetComponent<Collider2D>();
            buttonGlueAI.dumpsterArea = dumpsterArea.GetComponent<Collider2D>();
        }

        if (buttonRopeAI != null)
        {
            buttonRopeAI.toolUsageIndicator = toolIndicator;
            buttonRopeAI.toolUsageIndicator = toolIndicator;
            buttonRopeAI.conveyorCallback = this;
            buttonRopeAI.putawayArea = putAwayArea.GetComponent<Collider2D>();
            buttonRopeAI.dumpsterArea = dumpsterArea.GetComponent<Collider2D>();
        }

        pendingSpawnCount -= 1;
        spawnTimerCurrent = 0;
    }

    public GameObject SelectNextTool()
    {
        int i = UnityEngine.Random.Range(0, spwawnables.Count - 1);
        GameObject s = spwawnables[i];
        if (ropeGuaranteed)
        {
            s = ropePrefab;
            ropeGuaranteed = false;
        }

        return s;
    }

    public void AddPendingSpawns(int count)
    {
        pendingSpawnCount = pendingSpawnCount + count;
        if (GetToolCount() + pendingSpawnCount > maxSpawnedItems)
        {
            pendingSpawnCount = maxSpawnedItems - GetToolCount();
        }
    }

    public bool HasRope()
    {
        bool rope = false;
        foreach (var tool in myTools)
        {
            ToolPickupButtonRope r = tool.gameObject.GetComponent<ToolPickupButtonRope>();
            if (r != null)
            {
                rope = true;
            }
        }

        return rope;
    }

    public int GetToolCount()
    {
        return myTools.Count;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolConveyor : MonoBehaviour
{
    private GameState gameState;
    private List<GameObject> myTools;
    public GameObject toolIndicator;
    public TextBubbleManager textBubbleManager;

    private float spawnTimerCurrent;
    public float spawnTimer = 5;
    public float speed = 2;
    public int tutorialToolOverrideState = -1;

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
        bool showTutorial = tutorialToolOverrideState == 0;
        
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

        if (showTutorial)
        {
            ShowToolTutorials();
        }
    }

    public GameObject SelectNextTool()
    {
        int i = UnityEngine.Random.Range(0, spwawnables.Count - 1);
        GameObject s = spwawnables[i];
        if (ropeGuaranteed)
        {
            s = spwawnables[0];
            ropeGuaranteed = false;
        }

        if (tutorialToolOverrideState >= 0)
        {
            s = spwawnables[tutorialToolOverrideState];
            tutorialToolOverrideState = tutorialToolOverrideState - 1;
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
        //Debug.LogWarning("Adding pending tool count: "+pendingSpawnCount);
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

    public void ShowToolTutorials()
    {
        var items = Math.Min(3,GetToolCount());
        
        textBubbleManager.ClearDialogueBoxes();
        for (int i = 0; i < items; i++)
        {
            var tool = myTools[i];
            ToolPickupButtonRope buttonRopeAI = tool.GetComponent<ToolPickupButtonRope>();
            ToolPickupButtonGlue buttonGlueAI = tool.GetComponent<ToolPickupButtonGlue>();
            ToolPickupButtonTape buttonTapeAI = tool.GetComponent<ToolPickupButtonTape>();
            if (buttonTapeAI != null)
            {
                textBubbleManager.Say(tool.transform, "Tape can fasten cargo to each other.", 999999);
            }

            if (buttonGlueAI != null)
            {
                textBubbleManager.Say(tool.transform, "Glue can stick cargo directly to the truck.", 999999);
            }

            if (buttonRopeAI != null)
            {
                textBubbleManager.Say(tool.transform, "Use eyelets and lashing straps to fasten cargo.", 999999);
            }
        }
    }

    public int GetToolCount()
    {
        return myTools.Count;
    }
}
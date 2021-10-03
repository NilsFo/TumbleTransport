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

    public GameObject putAwayArea;
    public GameObject dumpsterArea;

    public SpriteRenderer mySprite;
    public float animationSpeed = 1;
    private float animationSpeedCurrent = 0;
    public int currentKeyFrame = 0;
    public List<Sprite> animationKeyFrames;

    private void Start()
    {
        toolIndicator.gameObject.SetActive(false);
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
            float newy = pos.y + speed * Time.deltaTime;

            if (newy >= maxToolDist)
            {
                newy = maxToolDist;
            }
            else
            {
                shouldMove = true;
            }

            tool.transform.localPosition = new Vector3(pos.x, newy, pos.z);
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
        newTool.transform.localPosition = new Vector3(creationXPos, 0, transform.localPosition.z - 1);

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

        spawnTimerCurrent = 0;
    }

    public GameObject SelectNextTool()
    {
        int i = UnityEngine.Random.Range(0, spwawnables.Count - 1);
        return spwawnables[i];
    }

    public int GetToolCount()
    {
        return myTools.Count;
    }
}
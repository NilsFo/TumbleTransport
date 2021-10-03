using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class ToolPickupButtonGlue : MonoBehaviour
{
    private GameState gameState;
    public SpriteRenderer mySprite;
    public bool isSelected;
    public GameObject GlueSplatPrefab;

    private Camera cam;
    private Vector3 selectionOrigin;
    private Vector3 selectionTarget;

    public Collider2D putawayArea;
    public Collider2D dumpsterArea;
    public GameObject toolUsageIndicator;
    public Vector2 toolFrameOffset = new Vector2();

    public ToolConveyor conveyorCallback;
    public int conveyorIndex;
    private bool interactable = true;
    private float timeHeld = 0;
    public float materialCost = 100f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        interactable = true;
        isSelected = false;
        gameState = FindObjectOfType<GameState>();
        selectionOrigin = new Vector3();
        selectionTarget = new Vector3();
        timeHeld = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Updating rendered sprite
        mySprite.enabled = !isSelected;

        // Checking if this tool is selected and if drag should be initiated
        if (Input.GetMouseButton(0) && isSelected)
        {
            selectionOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            bool isValid = CheckGlueValidPoints(selectionOrigin);
            if (isValid)
            {
                RequestToolUse(selectionOrigin);
                isSelected = false;
                interactable = false;
                gameState.currentSelectionState = GameState.SelectionState.None;
                toolUsageIndicator.gameObject.SetActive(false);
                conveyorCallback.Remove(gameObject);
                gameState.SubtractMaterialCost(materialCost, "Glue");
            }
        }

        if (Input.GetMouseButtonDown(0) && isSelected && timeHeld > .5)
        {
            Vector3 selectionTemp = cam.ScreenToWorldPoint(Input.mousePosition);
            if (putawayArea.OverlapPoint(selectionTemp))
            {
                // Putting the tool away
                print("glue put away");
                isSelected = false;
                interactable = false;
                gameState.currentSelectionState = GameState.SelectionState.None;
                toolUsageIndicator.gameObject.SetActive(false);

                Invoke("EnableInteractable", (float) (Time.smoothDeltaTime * 13.37));
            }

            if (dumpsterArea.OverlapPoint(selectionTemp))
            {
                // Deleting the selected tool
                print("delete");
                isSelected = false;
                interactable = false;
                gameState.currentSelectionState = GameState.SelectionState.None;
                toolUsageIndicator.gameObject.SetActive(false);
                conveyorCallback.Remove(gameObject);
                gameState.SubtractMaterialCost(materialCost, "Glue");
            }
        }

        // Checking if this tool is currently selected
        if (isSelected)
        {
            timeHeld = timeHeld + Time.deltaTime;
            if (toolUsageIndicator != null)
            {
                toolUsageIndicator.gameObject.SetActive(false);
            }

            Vector3 currentMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            //offsetPoint = currentSelectionPos;
            Vector3 offsetPoint = currentMousePos;
            toolUsageIndicator.gameObject.SetActive(true);
            offsetPoint.x = offsetPoint.x - toolFrameOffset.x;
            offsetPoint.y = offsetPoint.y - toolFrameOffset.y;
            offsetPoint.z = -6;
            toolUsageIndicator.gameObject.transform.position = offsetPoint;
        }
    }

    private void OnMouseUp()
    {
        Vector3 selectionTemp = cam.ScreenToWorldPoint(Input.mousePosition);
        if (!gameState.HasSomethingSelected() && interactable && GetComponent<Collider2D>().OverlapPoint(selectionTemp))
        {
            print("glue picked up");
            isSelected = true;
            selectionOrigin = new Vector3();
            selectionTarget = new Vector3();
            timeHeld = 0;

            toolUsageIndicator.gameObject.GetComponent<SpriteRenderer>().sprite = mySprite.sprite;
            toolUsageIndicator.gameObject.SetActive(true);
            gameState.currentSelectionState = GameState.SelectionState.Tool;
        }
        else
        {
            print("a click to be sure, but an illegal one");
        }
    }

    private void RequestToolUse(Vector3 start)
    {
        var splat = Instantiate(GlueSplatPrefab, FindObjectOfType<TruckBed>().transform);
        selectionOrigin.z = -0.5f;
        splat.transform.position = selectionOrigin;
        print("mew glue splat");
    }

    private bool CheckGlueValidPoints(Vector3 start)
    {
        GameObject truck = gameState.GetCurrentTruck();
        return truck.GetComponent<Truck>().canPlaceGlue(start);
    }

    public void EnableInteractable()
    {
        this.interactable = true;
    }
}
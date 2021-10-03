using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class ToolPickupButtonTape : MonoBehaviour
{
    private GameState gameState;
    public LashingStrap tapePrefab;
    public LashingStrap lashingStrapPreview;
    public float maximumDistance = 2.0f;
    public SpriteRenderer mySprite;
    public bool isSelected;

    private Camera cam;
    private bool isDragging;
    private Vector3 selectionOrigin;
    private Vector3 selectionTargetBestLength;
    private Vector3 selectionTarget;

    public Collider2D putawayArea;
    public Collider2D dumpsterArea;
    public GameObject toolUsageIndicator;
    public Vector2 toolFrameOffset = new Vector2();

    public ToolConveyor conveyorCallback;
    private bool interactable = true;
    public bool tooLong;
    public int materialCost = 100;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        interactable = true;
        isSelected = false;
        isDragging = false;
        gameState = FindObjectOfType<GameState>();
        selectionOrigin = new Vector3();
        selectionTarget = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        // Updating rendered sprite
        mySprite.enabled = !isSelected;

        // Checking if this tool is selected and if drag should be initiated
        if (Input.GetMouseButton(0) && isSelected && !isDragging)
        {
            selectionOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            List<GameObject> validAttachers = CheckToolTapeValidPoints(selectionOrigin);
            if (validAttachers.Count > 0)
            {
                isDragging = true;
                lashingStrapPreview = Instantiate(tapePrefab);
                lashingStrapPreview.GetComponent<Collider2D>().enabled = false;
                selectionOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                // Dragging nowhere special
                selectionOrigin = new Vector3();
            }
        }

        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            Vector3 selectionTemp = cam.ScreenToWorldPoint(Input.mousePosition);
            if (putawayArea.OverlapPoint(selectionTemp))
            {
                // Putting the tool away
                print("put away");
                isDragging = false;
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
                isDragging = false;
                isSelected = false;
                interactable = false;
                gameState.currentSelectionState = GameState.SelectionState.None;
                toolUsageIndicator.gameObject.SetActive(false);
                conveyorCallback.Remove(gameObject);
                gameState.SubtractMaterialCost(materialCost);
            }
        }

        // Checking if this tool is dragging and mouse has been released
        if (Input.GetMouseButtonUp(0) && isSelected && isDragging)
        {
            isSelected = false;
            isDragging = false;
            toolUsageIndicator.gameObject.SetActive(false);
            gameState.currentSelectionState = GameState.SelectionState.None;

            List<GameObject> validAttachers = CheckToolTapeValidPoints(selectionOrigin);
            Destroy(lashingStrapPreview.gameObject);
            print("");
            if (validAttachers.Count >= 1)
            {
                RequestToolUse(selectionOrigin, selectionTarget);
                conveyorCallback.Remove(gameObject);
                gameState.SubtractMaterialCost(materialCost);
            }
        }

        // Checking if this tool is currently selected
        if (isSelected)
        {
            if (toolUsageIndicator != null)
            {
                toolUsageIndicator.gameObject.SetActive(false);
            }

            Vector3 currentSelectionPos = cam.ScreenToWorldPoint(Input.mousePosition);
            // While it is dragging, draw a line
            if (isDragging)
            {
                float d = Vector3.Distance(selectionOrigin, currentSelectionPos);
                if (d < maximumDistance)
                {
                    selectionTarget = currentSelectionPos;
                    selectionTargetBestLength = currentSelectionPos;
                    tooLong = false;
                }
                else
                {
                    /*var angle = Vector3.Angle(selectionOrigin, currentSelectionPos);
                    
                    var x = maximumDistance * Mathf.Cos(angle * Mathf.Deg2Rad);
                    var y = maximumDistance * Mathf.Sin(angle * Mathf.Deg2Rad);
                    var newPosition = selectionOrigin;
                    newPosition.x += x;
                    newPosition.y += y;*/
                    
                    selectionTarget = selectionOrigin + (currentSelectionPos - selectionOrigin).normalized * maximumDistance;
                    tooLong = true;
                }

                print("Dragging from " + selectionOrigin + " to " + currentSelectionPos + ". Too long? " + tooLong);
                lashingStrapPreview.SetLashingStrap(selectionOrigin, selectionTarget);
            }
            // If it is not dragging, draw the current icon as an indicator
            else
            {
                //offsetPoint = currentSelectionPos;
                Vector3 offsetPoint = currentSelectionPos;
                toolUsageIndicator.gameObject.SetActive(true);
                offsetPoint.x = offsetPoint.x - toolFrameOffset.x;
                offsetPoint.y = offsetPoint.y - toolFrameOffset.y;
                offsetPoint.z = -6;
                toolUsageIndicator.gameObject.transform.position = offsetPoint;
            }
        }
    }

    private void OnMouseUp()
    {
        Vector3 selectionTemp = cam.ScreenToWorldPoint(Input.mousePosition);
        if (!gameState.HasSomethingSelected() && interactable && GetComponent<Collider2D>().OverlapPoint(selectionTemp))
        {
            print("Tool click");
            isSelected = true;
            selectionOrigin = new Vector3();
            selectionTarget = new Vector3();

            toolUsageIndicator.gameObject.GetComponent<SpriteRenderer>().sprite = mySprite.sprite;
            toolUsageIndicator.gameObject.SetActive(true);
            gameState.currentSelectionState = GameState.SelectionState.Tool;
        }
        else
        {
            print("a click to be sure, but an illegal one");
        }
    }

    private void RequestToolUse(Vector3 start, Vector3 end)
    {
        var strap = Instantiate(tapePrefab, FindObjectOfType<TruckBed>().transform);
        strap.SetLashingStrap(start, end);
    }

    private List<GameObject> CheckToolTapeValidPoints(Vector3 start)
    {
        List<GameObject> validAttachers = new List<GameObject>();
        GameObject truck = gameState.GetCurrentTruck();
        bool startValid = false;

        ToolAttachment[] attachments = truck.GetComponentsInChildren<ToolAttachment>(true);
        foreach (var attachment in attachments)
        {
            if (attachment.myType == ToolAttachment.AttachmentType.Cargo)
            {
                if (attachment.GetComponent<Collider2D>().OverlapPoint(start))
                {
                    startValid = true;
                    validAttachers.Add(attachment.gameObject);
                }
            }
        }

        if (!startValid)
        {
            validAttachers.Clear();
        }

        return validAttachers;
    }

    private List<GameObject> CheckToolTapeValidPoints(Vector3 start, Vector3 end)
    {
        List<GameObject> validAttachers = new List<GameObject>();
        GameObject truck = gameState.GetCurrentTruck();
        bool startValid = false;
        bool endValid = false;

        ToolAttachment[] attachments = truck.GetComponentsInChildren<ToolAttachment>(true);
        foreach (var attachment in attachments)
        {
            if (attachment.myType == ToolAttachment.AttachmentType.Cargo)
            {
                if (attachment.GetComponent<Collider2D>().OverlapPoint(start))
                {
                    startValid = true;
                    validAttachers.Add(attachment.gameObject);
                }

                if (attachment.GetComponent<Collider2D>().OverlapPoint(end))
                {
                    endValid = true;
                    validAttachers.Add(attachment.gameObject);
                }
            }
        }

        if (startValid && endValid)
        {
            if (validAttachers[0] == validAttachers[1])
            {
                validAttachers.Clear();
            }
        }

        return validAttachers;
    }

    public void EnableInteractable()
    {
        this.interactable = true;
    }
}

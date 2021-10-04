using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class ToolPickupButtonRope : MonoBehaviour
{
    private GameState gameState;
    public LashingStrap lashingStrapPrefab;
    public LashingStrap lashingStrapPreview;
    public SpriteRenderer mySprite;
    public bool isSelected;

    private Camera cam;
    private bool isDragging;
    private Vector3 selectionOrigin;
    private Vector3 selectionTarget;

    public Collider2D putawayArea;
    public Collider2D dumpsterArea;
    public GameObject toolUsageIndicator;
    public Vector2 toolFrameOffset = new Vector2();

    public ToolConveyor conveyorCallback;
    public int conveyorIndex;
    public float materialCost = 100;
    public string materialNameInScore = "Spend Rope";
    private bool interactable = true;
    private float timeHeld = 0;
    private bool pickedUpThisFrame = false;

    public AudioClip ropeEndSound;

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
        timeHeld = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Updating rendered sprite
        mySprite.enabled = !isSelected;
        if (pickedUpThisFrame)
        {
            pickedUpThisFrame = false;
            return;
        }

        // Checking if this tool is selected and if drag should be initiated
        if (Input.GetMouseButton(0) && isSelected && !isDragging)
        {
            selectionOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            List<GameObject> validAttachers = CheckToolRopeValidPoints(selectionOrigin);
            if (validAttachers.Count > 0)
            {
                isDragging = true;
                selectionOrigin = validAttachers[0].GetComponent<Collider2D>().bounds.center;
                lashingStrapPreview = Instantiate(lashingStrapPrefab);
                lashingStrapPreview.GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                // Dragging nowhere special
            }
        }

        if (Input.GetMouseButtonDown(0) && isSelected && timeHeld > .5)
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

                Invoke("EnableInteractable", (0.005f));
            }

            if (dumpsterArea.OverlapPoint(selectionTemp))
            {
                // Deleting the selected tool
                gameState.tutorialHasDeletedAtLeastOnce = true;
                isDragging = false;
                isSelected = false;
                interactable = false;
                gameState.currentSelectionState = GameState.SelectionState.None;
                toolUsageIndicator.gameObject.SetActive(false);
                conveyorCallback.Remove(gameObject);
                conveyorCallback.AddPendingSpawns(1);
                CreateFloatingText(dumpsterArea.bounds.center);
                gameState.SubtractMaterialCost(materialCost, materialNameInScore, mySprite.sprite);
            }
        }

        // Checking if this tool is dragging and mouse has been released
        if (Input.GetMouseButtonUp(0) && isSelected && isDragging)
        {
            isSelected = false;
            isDragging = false;
            toolUsageIndicator.gameObject.SetActive(false);
            gameState.currentSelectionState = GameState.SelectionState.None;

            List<GameObject> validAttachers = CheckToolRopeValidPoints(selectionOrigin, selectionTarget);
            Destroy(lashingStrapPreview.gameObject);
            if (validAttachers.Count >= 2)
            {
                // Updating centers
                Vector3 firstCenter = validAttachers[0].GetComponent<Collider2D>().bounds.center;
                Vector3 secondCenter = validAttachers[1].GetComponent<Collider2D>().bounds.center;

                RequestToolUse(firstCenter, secondCenter);
                CreateFloatingText(selectionTarget);
                conveyorCallback.Remove(gameObject);
                gameState.SubtractMaterialCost(materialCost, materialNameInScore, mySprite.sprite);
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

            Vector3 currentSelectionPos = cam.ScreenToWorldPoint(Input.mousePosition);
            // While it is dragging, draw a line
            if (isDragging)
            {
                // print("Dragging rope from "+selectionOrigin + " to "+currentSelectionPos);
                lashingStrapPreview.SetLashingStrap(selectionOrigin, currentSelectionPos);
                selectionTarget = currentSelectionPos;
            }
            // If it is not dragging, draw the current icon as an indicator
            else
            {
                //offsetPoint = currentSelectionPos;
                Vector3 offsetPoint = currentSelectionPos;
                offsetPoint.x = offsetPoint.x - toolFrameOffset.x;
                offsetPoint.y = offsetPoint.y - toolFrameOffset.y;
                offsetPoint.z = -6;
                toolUsageIndicator.gameObject.transform.position = offsetPoint;
                toolUsageIndicator.gameObject.SetActive(true);
            }
        }
    }

    private void OnMouseDown()
    {
        Vector3 selectionTemp = cam.ScreenToWorldPoint(Input.mousePosition);
        if (!gameState.HasSomethingSelected() && interactable && GetComponent<Collider2D>().OverlapPoint(selectionTemp))
        {
            print("Tool click");
            pickedUpThisFrame = true;
            isSelected = true;
            selectionOrigin = new Vector3();
            selectionTarget = new Vector3();
            timeHeld = 0;

            toolUsageIndicator.gameObject.GetComponent<SpriteRenderer>().sprite = mySprite.sprite;
            // toolUsageIndicator.gameObject.SetActive(true);
            gameState.currentSelectionState = GameState.SelectionState.Tool;
        }
        else
        {
            print("a click to be sure, but an illegal one");
        }
    }

    private void RequestToolUse(Vector3 start, Vector3 end)
    {
        var strap = Instantiate(lashingStrapPrefab, FindObjectOfType<TruckBed>().transform);
        var ac = strap.GetComponent<AudioSource>();
        ac.clip = ropeEndSound;
        ac.Play();
        strap.SetLashingStrap(start, end);
        gameState.tutorialHasTooledAtLeastOnce = true;
        gameState.startBT.AnimateButtonText();
    }

    private List<GameObject> CheckToolRopeValidPoints(Vector3 start)
    {
        List<GameObject> validAttachers = new List<GameObject>();
        GameObject truck = gameState.GetCurrentTruck();
        bool startValid = false;

        ToolAttachment[] attachments = truck.GetComponentsInChildren<ToolAttachment>(true);
        foreach (var attachment in attachments)
        {
            if (attachment.myType == ToolAttachment.AttachmentType.Eyelet)
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

    private List<GameObject> CheckToolRopeValidPoints(Vector3 start, Vector3 end)
    {
        List<GameObject> validAttachers = new List<GameObject>();
        GameObject truck = gameState.GetCurrentTruck();
        bool startValid = false;
        bool endValid = false;

        // TODO ref truck here

        ToolAttachment[] attachments = truck.GetComponentsInChildren<ToolAttachment>(true);
        foreach (var attachment in attachments)
        {
            if (attachment.myType == ToolAttachment.AttachmentType.Eyelet)
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


    public void CreateFloatingText(Vector3 textPos)
    {
        GameObject textGO = Instantiate(floatingTextPrefab);
        textPos.z = -9;
        textGO.transform.position = textPos;
        FloatingText text = textGO.GetComponent<FloatingText>();

        text.text = "- $" + materialCost;
        text.textColor = new Color(201 / 255f, 48 / 255f, 56 / 255f);

        text.duration = 1.5f;
        text.velocity = Vector3.up * 1.5f;
        text.fontSize = 32;

        // GameObject.Find("/Audio/KachingSound").GetComponent<AudioSource>().Play();
    }

    public GameObject floatingTextPrefab;
}

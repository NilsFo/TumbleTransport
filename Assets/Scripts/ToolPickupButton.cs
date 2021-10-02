using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class ToolPickupButton : MonoBehaviour
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

    public GameObject toolUsageIndicator;
    public Vector2 toolFrameOffset = new Vector2();
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
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
        if (Input.GetMouseButton(0) && isSelected &&!isDragging)
        {
            isDragging = true;
            selectionOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            lashingStrapPreview = Instantiate(lashingStrapPrefab);
            lashingStrapPreview.GetComponent<Collider2D>().enabled = false;
        }

        // Checking if this tool is dragging and mouse has been released
        if (Input.GetMouseButtonUp(0) && isSelected && isDragging)
        {
            isSelected = false;
            isDragging = false;
            toolUsageIndicator.gameObject.SetActive(false);
            gameState.currentSelectionState = GameState.SelectionState.None;
            Destroy(lashingStrapPreview.gameObject);
            RequestToolUse(selectionOrigin,selectionTarget);
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
//                print("Dragging from "+selectionOrigin + " to "+currentSelectionPos);
                Debug.DrawLine(selectionOrigin,currentSelectionPos);
                lashingStrapPreview.SetLashingStrap(selectionOrigin, currentSelectionPos);
                selectionTarget = currentSelectionPos;
            }
            // If it is not dragging, draw the current icon as an indicator
            else
            {
                print("following");
                //offsetPoint = currentSelectionPos;
                Vector3 offsetPoint = currentSelectionPos;//- toolFrameOffset;
                toolUsageIndicator.gameObject.SetActive(true);
                offsetPoint.x = offsetPoint.x - toolFrameOffset.x;
                offsetPoint.y = offsetPoint.y - toolFrameOffset.y;
                offsetPoint.z = -6;
                toolUsageIndicator.gameObject.transform.position = offsetPoint;
            }
        }
        //toolUsageIndicator.gameObject.SetActive(true);
    }

    private void OnMouseUp()
    {
        if (!gameState.HasSomethingSelected())
        {
            isSelected = true;
            selectionOrigin = new Vector3();
            selectionTarget = new Vector3();
            
            toolUsageIndicator.gameObject.GetComponent<SpriteRenderer>().sprite = mySprite.sprite;
            toolUsageIndicator.gameObject.SetActive(true);
            gameState.currentSelectionState = GameState.SelectionState.Tool;
        }
    }

    private void RequestToolUse(Vector3 start, Vector3 end) {
        var strap = Instantiate(lashingStrapPrefab, FindObjectOfType<TruckBed>().transform);
        strap.SetLashingStrap(start, end);
    }
    
    
}

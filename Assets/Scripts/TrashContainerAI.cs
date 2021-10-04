using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrashContainerAI : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite openSprite;
    public Sprite closedSprite;
    public SpriteRenderer myRenderer;
    public Collider2D myCollider;
    public TMP_Text myText;

    private Camera cam;
    private GameState gameState;

    void Start()
    {
        cam = Camera.main;
        myRenderer.sprite = closedSprite;
        gameState = FindObjectOfType<GameState>();
        myText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (myCollider.OverlapPoint(mousePos) && gameState.currentSelectionState != GameState.SelectionState.Cargo)
        {
            myRenderer.sprite = openSprite;

            if (!gameState.tutorialHasDeletedAtLeastOnce)
            {
                myText.enabled = true;
            }
        }
        else
        {
            myRenderer.sprite = closedSprite;
            myText.enabled = false;
        }

        if (gameState.tutorialHasLoadedTruckAtLeastOnce && !gameState.tutorialHasDepartedAtLeastOnce)
        {
            myText.enabled = true;
        }
        
    }
}
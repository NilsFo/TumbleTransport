using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashContainerAI : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite openSprite;
    public Sprite closedSprite;
    public SpriteRenderer myRenderer;
    public Collider2D myCollider;

    private Camera cam;
    private GameState gameState;

    void Start()
    {
        cam = Camera.main;
        myRenderer.sprite = closedSprite;
        gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (myCollider.OverlapPoint(mousePos) && gameState.currentSelectionState != GameState.SelectionState.Cargo)
        {
            myRenderer.sprite = openSprite;
        }
        else
        {
            myRenderer.sprite = closedSprite;
        }
    }
}
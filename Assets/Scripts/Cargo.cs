using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Cargo : MonoBehaviour {
    public Collider2D coll;
    public SpriteRenderer sprite;

    public bool taped;

    public bool fastened;
    public bool grabbed;

    public GameObject floatingTextPrefab;
    private GameObject lastFloatingText;
    public CargoScriptableObject dataObject;

    private bool flying;
    private float _flyingTimer = 0f;
    private float _maxFlyingTimer = 5f;
    
    private Vector3 grabPivot;
    private Vector3 grabbedFrom;
    private Quaternion grabbedFromRot;
    private Camera cam;
    private ContactFilter2D _contactFilter;

    private GameState _gameState;

    private Truck truck;
    private bool _droppable;
    // Start is called before the first frame update
    void Start()
    {
        taped = false;
        cam = Camera.main;
        _contactFilter = new ContactFilter2D {
            layerMask = LayerMask.GetMask(new String[]{"CrateDropArea", "CrateForbiddenArea"}),
            useTriggers = true
        };
        _gameState = FindObjectOfType<GameState>();
    }

    void Update() {
        if (_flyingTimer > _maxFlyingTimer)
        {
            Destroy(gameObject);
        }
        else if (flying) {
            transform.position += Vector3.down * Time.deltaTime * 5f;
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 180f * Time.deltaTime);
            _flyingTimer += Time.deltaTime;
        }
        else if (!fastened & grabbed) {
            _droppable = false;
            var pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -5;
            //Debug.Log(pos);
            transform.position = pos - transform.rotation * grabPivot;

            var colliderResults = new List<Collider2D>();
            var collisions = coll.OverlapCollider(_contactFilter, colliderResults);
            if (colliderResults.Count > 0) {
                _droppable = true;
                Truck _truck = null;
                for (var i = 0; i < collisions; i++) {
                    var colliderResult = colliderResults [i];
                    if (!colliderResult.gameObject.layer.Equals(LayerMask.NameToLayer("CrateDropArea"))  // Can only place on CrateDropAreas
                            && !(colliderResult.gameObject.layer.Equals(LayerMask.NameToLayer("Straps")) && colliderResult.GetComponent<Glue>() != null))  // Or on Glue 
                    {
                        _droppable = false;
                    } else if (colliderResult.gameObject.layer.Equals(LayerMask.NameToLayer("CrateDropArea"))
                            && !colliderResult.gameObject.tag.Equals("Truck")) {
                        // Sonderregel für rechts droppen
                        _droppable = true;
                    }
                    if (colliderResult.gameObject.CompareTag("Truck")) {
                        _truck = colliderResult.GetComponentInParent<Truck>();
                    }
                }
                if (_droppable) {
                    truck = _truck;
                    sprite.color = Color.white;
                }
                else {
                    sprite.color = Color.red;
                }
            } else {
                sprite.color = Color.red;
            }
        }

        if (grabbed && Input.GetMouseButtonDown(1)) {
            Debug.Log("Rotate");
            transform.rotation *= Quaternion.Euler(0,0,-90f);
            _gameState.tutorialHasRotatedAtLeastOnce = true;
            
            if (lastFloatingText != null)
            {
                Destroy(lastFloatingText.gameObject);
            }
        }
    }
    // Update is called once per frame
    void OnMouseDown() {
        if(_gameState.HasSomethingSelected())
            return;
        
        List<Collider2D> results = new List<Collider2D>();
        if (coll.OverlapCollider(new ContactFilter2D().NoFilter(), results) > 0) {
            foreach (var result in results) {
                if (result.gameObject.layer == LayerMask.NameToLayer("Straps")) {
                    Debug.Log("Can't grab what is already fastened");
                    ShowFloatingText("It's fastened tight.");
                    _gameState.boxDropSound.Play();
                    return;
                }
            }
        }
        if (taped) {
            _gameState.boxDropSound.Play();
            return;
        }
        grabbed = true;
        grabbedFrom = transform.position;
        grabbedFromRot = transform.rotation;
        grabPivot = transform.worldToLocalMatrix.MultiplyPoint(cam.ScreenToWorldPoint(Input.mousePosition));
        grabPivot.z = 0;
        _gameState.currentSelectionState = GameState.SelectionState.Cargo;
        _gameState.boxPickupSound.Play();

        Destroy(GetComponent<CargoSpawnAnimator>());
        
        // Displaying rotation tutorial reminder
        if (!_gameState.tutorialHasRotatedAtLeastOnce && _gameState.tutorialHasLoadedTruckAtLeastOnce)
        {
            ShowFloatingText("Rotate cargo using right click.");
        }
    }
    void OnMouseUp() {
        if (!grabbed)
            return;
        grabbed = false;
        sprite.color = Color.white;
        _gameState.currentSelectionState = GameState.SelectionState.None;
        if (_droppable) {
            if (truck != null)
            {
                transform.SetParent(truck.GetComponentInChildren<TruckBed>().transform);
                if (_gameState.tutorialRunning && !_gameState.tutorialHasLoadedTruckAtLeastOnce)
                {
                    _gameState.toolConveyor.maxSpawnedItems = 4;
                    _gameState.toolConveyor.AddPendingSpawns(4);
                }
                _gameState.tutorialHasLoadedTruckAtLeastOnce = true;
            }
            else {
                transform.SetParent(null);
            }
            var t = transform.position;
            t.z = FindObjectOfType<SpawnCargo>().spawnPoint.transform.position.z;
            transform.position = t;
        } else {
            transform.position = grabbedFrom;
            transform.rotation = grabbedFromRot;
        }
        
        _gameState.boxDropSound.Play();
    }

    public void ThrowCargo() {
        transform.SetParent(null);
        var t = transform.position;
        t.z = -7;
        transform.position = t;
        flying = true;
        
        _gameState.boxDropSound.Play();
    }

    public void ShowFloatingText(string message)
    {   
        if (lastFloatingText != null)
        {
            Destroy(lastFloatingText.gameObject);
        }
        
        GameObject textGO = Instantiate(floatingTextPrefab,gameObject.transform);
        Vector3 textPos = textGO.transform.position;
        textPos.z = -9;
        textGO.transform.position = textPos;
        textGO.transform.rotation = Quaternion.identity;
        lastFloatingText = textGO;
        FloatingText text = textGO.GetComponent<FloatingText>();

        text.text = message;
        text.textColor = Color.white;

        text.duration = 0.95f;
        text.velocity = Vector3.up * 0.69f;
        text.fontSize = 12;
    }

}

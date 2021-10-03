using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Cargo : MonoBehaviour {
    public Collider2D coll;
    public SpriteRenderer sprite;

    public bool taped;

    public bool fastened;
    public bool grabbed;

    public CargoScriptableObject dataObject;

    private bool flying;
    private float _flyingTimer = 0f;
    private float _maxFlyingTimer = 5f;
    
    private Vector3 grabPivot;
    private Vector3 grabbedFrom;
    private Camera cam;
    private ContactFilter2D _contactFilter;

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
            pos.z = -1;
            //Debug.Log(pos);
            transform.position = pos - grabPivot;

            var colliderResults = new List<Collider2D>();
            var collisions = coll.OverlapCollider(_contactFilter, colliderResults);
            if (colliderResults.Count > 0) {
                _droppable = true;
                Truck _truck = null;
                for (var i = 0; i < collisions; i++) {
                    var colliderResult = colliderResults [i];
                    if (!colliderResult.gameObject.layer.Equals(LayerMask.NameToLayer("CrateDropArea"))) {
                        _droppable = false;
                        break;
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
        
    }
    // Update is called once per frame
    void OnMouseDown() {
        List<Collider2D> results = new List<Collider2D>();
        if (coll.OverlapCollider(new ContactFilter2D().NoFilter(), results) > 0) {
            foreach (var result in results) {
                if (result.gameObject.layer == LayerMask.NameToLayer("Straps")) {
                    Debug.Log("Can't grab what is already fastened");
                    return;
                }
            }
        }
        if (taped) {
            return;
        }
        grabbed = true;
        grabbedFrom = transform.position;
        grabPivot = transform.worldToLocalMatrix.MultiplyPoint(cam.ScreenToWorldPoint(Input.mousePosition));
        grabPivot.z = 0;
    }
    void OnMouseUp() {
        grabbed = false;
        sprite.color = Color.white;
        if (_droppable) {
            

            if (truck != null)
                transform.SetParent(truck.GetComponentInChildren<TruckBed>().transform);
            else {
                transform.SetParent(null);
            }
        } else {
            transform.position = grabbedFrom;
        }
    }

    public void ThrowCargo() {
        transform.SetParent(null);
        var t = transform.position;
        t.z = -7;
        transform.position = t;
        flying = true;
    }

    public void OnTapeAttached(Cargo partner)
    {
        // TODO implement
        taped = true;
        
        print("i am a cargo. i just got taped");
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Cargo : MonoBehaviour {
    public Collider2D coll;
    public SpriteRenderer sprite;
    
    public bool grabbed;
    private Vector3 grabPivot;
    private Camera cam;
    private ContactFilter2D _contactFilter;

    private Truck truck;
    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
        _contactFilter = new ContactFilter2D {
            layerMask = LayerMask.GetMask(new String[]{"CrateDropArea", "CrateForbiddenArea"}),
            useTriggers = true
        };
    }

    void Update() {
        if (grabbed) {
            
            var pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -5;
            //Debug.Log(pos);
            transform.position = pos - grabPivot;

            var colliderResults = new Collider2D[5];
            var collisions = coll.OverlapCollider(_contactFilter, colliderResults);
            if (collisions > 0) {
                bool droppable = true;
                Truck _truck = null;
                for (var i = 0; i < collisions; i++) {
                    var colliderResult = colliderResults [i];
                    if (colliderResult.gameObject.layer == LayerMask.NameToLayer("CrateForbiddenArea")) {
                        droppable = false;
                        break;
                    } 
                    if (colliderResult.gameObject.CompareTag("Truck")) {
                        _truck = colliderResult.GetComponentInParent<Truck>();
                    }
                }
                if (droppable) {
                    truck = _truck;
                    sprite.color = Color.green;
                }
                else {
                    sprite.color = Color.white;
                }
            } else {
                sprite.color = Color.white;
            }
        }
        
    }
    // Update is called once per frame
    void OnMouseDown() {
        grabbed = true;
        grabPivot = transform.worldToLocalMatrix.MultiplyPoint(cam.ScreenToWorldPoint(Input.mousePosition));
        grabPivot.z = 0;
        Debug.Log(grabPivot);
    }
    void OnMouseUp() {
        grabbed = false;
        if(truck != null)
            transform.SetParent(truck.GetComponentInChildren<TruckBed>().transform);
        else {
            transform.SetParent(null);
        }
    }
}

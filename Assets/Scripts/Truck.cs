using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour {
    public Animator truckAnimator;
    
    public enum TruckState
    {
        Spawned,
        In,
        Stay,
        Out,
        Traveling
    }
    
    private TruckState _state = TruckState.Spawned;
    private TruckState _lastState = TruckState.Spawned;
    
    public TruckState State
    {
        get => _state;
    } 
    
    // Start is called before the first frame update
    void Start()
    {
        _state = TruckState.In;
    }

    // Update is called once per frame
    void Update() {
        if (_state != _lastState)
        {
            if (_state == TruckState.In)
            {
                truckAnimator.Play("TruckEnter");
                Invoke(nameof(SetStay), 2.0f);
            } 
            else if (_state == TruckState.Out)
            {
                truckAnimator.Play("TruckExit");
                Invoke(nameof(ThrowAllCargo), 0.8f);
                Invoke(nameof(SetTraveling), 2.0f);
                BuildGraph();
            }
            _lastState = _state;
        }
    }
    
    public void BuildGraph() {
        var straps = GetComponentsInChildren<LashingStrap>();
        var cargo = GetComponentsInChildren<Cargo>();
        Debug.Log("Found " + cargo.Length + " cargo on truck");
        Node rootNode = new Node(null);

        foreach (var strap in straps) {
            var strapnode = new Node(strap.gameObject);
            rootNode.nodes.Add(strapnode);
            Debug.Log("Found Strap " + strap + " on Truck");
            var coll = strap.GetComponent<Collider2D>();
            
            var collResults = new List<Collider2D>();
            
            var nCollisions = coll.OverlapCollider(new ContactFilter2D() {
                layerMask = LayerMask.NameToLayer("Crate")
            }, collResults);
            
            
            foreach (var c in collResults) {
                var cgo = c.GetComponent<Cargo>();
                if (cgo != null) {
                    Debug.Log("Found Cargo " + cgo + " on Strap");
                    strapnode.nodes.Add(new Node(cgo.gameObject));
                }
            }
        }

        var result = traverseTree(rootNode);
        foreach (var c in cargo) {
            if (!result.Contains(c.gameObject)) {
                c.fastened = false;
            } else {
                c.fastened = true;
            }
        }
        
        
    }

    private List<GameObject> traverseTree(Node rootNode) {
        Queue<Node> discovered = new Queue<Node>();
        List<GameObject> found = new List<GameObject>();
        discovered.Enqueue(rootNode);
        while (discovered.Count > 0) {
            Node active = discovered.Dequeue();
            found.Add(active.gameObject);
            foreach (var n in active.nodes) {
                discovered.Enqueue(n);
            }
        }

        return found;
    }

    public void ThrowAllCargo() {
        
        var cargo = GetComponentsInChildren<Cargo>();
        foreach (var c in cargo) {
            if(!c.fastened)
                c.ThrowCargo();
        }
    }

    public void Dispatch()
    {
        _state = TruckState.Out;
    }

    private void SetStay()
    {
        _state = TruckState.Stay;
    }
    
    private void SetTraveling()
    {
        _state = TruckState.Traveling;
    }
}

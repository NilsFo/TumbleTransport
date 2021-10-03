using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Truck : MonoBehaviour {
    public Animator truckAnimator;

    public List<GameObject> eyeletsLeft;
    public List<GameObject> eyeletsRight;
    public int minEyeletsLeft = 1;
    public int minEyeletsRight = 1;

    public Transform dialoguePosition;

    private float _quoteWaitingTimer;
    
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

    public String destination;
    public String[] possibleDestinations = {
        "Downtown Coolsville",
        "Central Airport, Terminal 3",
        "Jammer Loggin Corp.",
        "49th East, Ludum Dare Ave.",
        "Southwest Garbage Dump",
        "Nevergreen Shipping Co.",
        "Springfield",
        "Gorleben",
        "Goodsprings",
        "Timbuktu",
        "1600 Pennsylvania Avenue",
        "Bochum-Wattenscheid, NRW",
        "Marl",
        "Whiterun",
        "Pallet Town",
        "New Londo",
        "Tattoine",
        "Umbrella Corp. Logistics",
        "Eyjafjallajökull",
        "Pripyat, UKR",
        "Weyland-Yutani Corp.",
        "221B Baker Street, London",
        "Ableben Funeral Home",
        "Chicago",
        "San Diego",
        "Innsmouth",
        "Truth or Consequences, NM",
        "Qo'noS",
        "Berlin",
        "Rotterdam",
        "Tokyo",
        "Sidney",
        "Hong Kong",
        "3838 Piermont Drive, Albuquerque, NM",
        "Wacken",
        "The Boneyard",
        "Großpösna, Lower-Saxony",
        "Necropolis",
        "Vienna",
        "Stockholm",
        "Rome",
        "Stratholme",
        "Nowhere in particular",
        "Just around the corner",
        "Hudson River",
        "Reykjavík",
        "Bermuda Triangle",
        "Blackreach",
        "Québec",
        "Seattle",
        "Sen's Building Co.",
        "Teufort",
        "Outset Island",
        "Bob-Omb Battlefield"
    };
    
    public TruckState State
    {
        get => _state;
    } 
    
    // Start is called before the first frame update
    void Start()
    {
        _state = TruckState.In;
        
        // Removing eyelets if needed
        for (int i = minEyeletsLeft; i < eyeletsLeft.Count; i++)
        {
            if (UnityEngine.Random.Range(0,1)==0)
            {
                GameObject eye = eyeletsLeft[UnityEngine.Random.Range(0, eyeletsLeft.Count)];
                eyeletsLeft.Remove(eye);
                Destroy(eye);
            }
        }
        for (int i = minEyeletsRight; i < eyeletsRight.Count; i++)
        {
            if (UnityEngine.Random.Range(0,1)==0)
            {
                GameObject eye = eyeletsRight[UnityEngine.Random.Range(0, eyeletsRight.Count)];
                eyeletsRight.Remove(eye);
                Destroy(eye);
            }
        }
        
        SetQuoteWaitingTimer();
        FindObjectOfType<TruckDialogueManager>().radioPos = dialoguePosition;

        destination = possibleDestinations [Random.Range(0, possibleDestinations.Length)];
        GameObject.Find("/Canvas/DestinationDisplay").GetComponent<TextMeshProUGUI>().text = destination;
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
        _quoteWaitingTimer -= Time.deltaTime;
        if (_quoteWaitingTimer < 0) {
            FindObjectOfType<TruckDialogueManager>().ReadRandomWaitingQuote();
            SetQuoteWaitingTimer();
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
        if (_state == TruckState.Stay)
        {
            _state = TruckState.Out;
        }
    }

    private void SetStay()
    {
        _state = TruckState.Stay;
    }
    
    private void SetTraveling()
    {
        _state = TruckState.Traveling;
    }

    private void SetQuoteWaitingTimer() {
        _quoteWaitingTimer = Random.Range(10f, 20f);
    }
}

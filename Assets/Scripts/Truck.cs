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
    public List<GameObject> eyeletsExtra;
    public int minEyeletsLeft = 1;
    public int minEyeletsRight = 1;
    public int minEyeletsExtra = 0;

    public Transform dialoguePosition;

    private float _quoteWaitingTimer;
    
    public TruckScriptableObject truckData;
    
    public List<CargoScriptableObject> fastenedCargo;
    public List<CargoScriptableObject> thrownCargo;
    public List<CargoScriptableObject> allCargo;

    public AudioSource truckLeaveAudio;
    
    public enum TruckState
    {
        Spawned,
        In,
        Stay,
        Out,
        Traveling
    }

    private GameState gameState;
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
        "New York",
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
        "Atlantis",
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
        "Libery City",
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
        gameState = FindObjectOfType<GameState>();
        
        fastenedCargo = new List<CargoScriptableObject>();
        thrownCargo = new List<CargoScriptableObject>();
        allCargo = new List<CargoScriptableObject>();
        
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
        for (int i = minEyeletsExtra; i < eyeletsExtra.Count; i++)
        {
            if (UnityEngine.Random.Range(0,1)==0)
            {
                GameObject eye = eyeletsExtra[UnityEngine.Random.Range(0, eyeletsExtra.Count)];
                eyeletsExtra.Remove(eye);
                Destroy(eye);
            }
        }
        
        SetQuoteWaitingTimer();
        FindObjectOfType<TruckDialogueManager>().radioPos = dialoguePosition;

        destination = possibleDestinations [Random.Range(0, possibleDestinations.Length)];
        GameObject.Find("/Canvas/DestinationDisplay").GetComponent<TextMeshProUGUI>().text = destination;

        GameObject.Find("/Audio/TruckArriveAudio").GetComponent<AudioSource>().Play();
        truckLeaveAudio = GameObject.Find("/Audio/TruckLeaveAudio").GetComponent<AudioSource>();
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
                truckLeaveAudio.Play();
                Invoke(nameof(ThrowAllCargo), 0.8f);
                Invoke(nameof(SetTraveling), 2.0f);
                BuildGraph();
                gameState.tutorialHasDepartedAtLeastOnce = true;
                gameState.startBT.animationPlaying = false;
            }
            _lastState = _state;
        }
        
        _quoteWaitingTimer -= Time.deltaTime;
        if (gameState.forceNextDriverQuote)
        {
            _quoteWaitingTimer = -10;
            gameState.forceNextDriverQuote = false;
            FindObjectOfType<TruckDialogueManager>().textBubbleManager.ClearDialogueBoxes();
        }
        
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
        Dictionary<GameObject, Node> found = new Dictionary<GameObject, Node>();

        foreach (var strap in straps) {
            var strapnode = new Node(strap.gameObject);
            
            if(strap.fixedOnTruck)
                rootNode.nodes.Add(strapnode);
            
            Debug.Log("Found Strap or Glue " + strap + " on Truck");
            var coll = strap.GetComponent<Collider2D>();

            var collResults = new List<Collider2D>();

            var nCollisions = coll.OverlapCollider(new ContactFilter2D() {
                layerMask = LayerMask.NameToLayer("Crate")
            }, collResults);

            // This is a lashing strap or glue
            foreach (var c in collResults) {
                var cgo = c.GetComponent<Cargo>();
                if (cgo != null) {
                    Debug.Log("Found Cargo " + cgo + " on Strap or Glue");
                    
                    var node = new Node(cgo.gameObject);
                    if (found.ContainsKey(cgo.gameObject)) {
                        node = found [cgo.gameObject];
                    } else {
                        found.Add(cgo.gameObject, node);
                    }
                    strapnode.nodes.Add(node);
                    node.nodes.Add(strapnode);
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
        HashSet<Node> visited = new HashSet<Node>();
        discovered.Enqueue(rootNode);
        while (discovered.Count > 0) {
            Node active = discovered.Dequeue();
            visited.Add(active);
            found.Add(active.gameObject);
            foreach (var n in active.nodes) {
                if(!visited.Contains(n))
                    discovered.Enqueue(n);
            }
        }

        return found;
    }

    public bool canPlaceGlue(Vector3 position) {
        bool canPlace = false;
        foreach (var collider in GetComponentsInChildren<Collider2D>()) {
            if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("CrateDropArea"))) {
                if (collider.OverlapPoint(position))
                    canPlace = true;
            } else if(collider.gameObject.layer.Equals(LayerMask.NameToLayer("CrateForbiddenArea")))
            {
                if (collider.OverlapPoint(position))
                    return false;
            }
        }
        return canPlace;
    }

    public void ThrowAllCargo() {
        var cargo = GetComponentsInChildren<Cargo>();
        foreach (var c in cargo) {
            allCargo.Add(c.dataObject);
            if (!c.fastened)
            {
                c.ThrowCargo();
                thrownCargo.Add(c.dataObject);
            }
            else
            {
                fastenedCargo.Add(c.dataObject);
            }
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

        if (!gameState.tutorialHasLoadedTruckAtLeastOnce)
        {
            _quoteWaitingTimer = _quoteWaitingTimer * .8f;
        }

        if (!gameState.tutorialHasLoadedTruckAtLeastOnce || !gameState.tutorialHasDepartedAtLeastOnce || !gameState.tutorialHasTooledAtLeastOnce)
        {
            _quoteWaitingTimer = _quoteWaitingTimer * .6f;
        }
    }
}

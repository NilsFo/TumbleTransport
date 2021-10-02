using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour {
    public Animator truckAnimator;

    private float timer = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        truckAnimator.Play("TruckEnter");
    }

    // Update is called once per frame
    void Update() {
        timer -= Time.deltaTime;
        if(timer < 0)
            truckAnimator.Play("TruckExit");
    }
}

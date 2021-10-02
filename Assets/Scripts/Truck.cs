using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour {
    public Animator truckAnimator;
    
    public enum TruckState
    {
        In, Out, Stay 
    }
    
    private TruckState _state = TruckState.In;
    
    

    private float timer = 12.5f;
    // Start is called before the first frame update
    void Start()
    {
        //TODO set Truckspot in Gamestate
        truckAnimator.Play("TruckEnter");
        _state = TruckState.In;
    }

    // Update is called once per frame
    void Update() {
        timer -= Time.deltaTime;
        if(timer < 0)
            truckAnimator.Play("TruckExit");

        if (truckAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !truckAnimator.IsInTransition(0))
        {
            if (_state == TruckState.In)
            {
                _state = TruckState.Stay;
            }
            else if(_state == TruckState.Out)
            {
                //Clean Gamestate Truck
            }
        }
    }
}

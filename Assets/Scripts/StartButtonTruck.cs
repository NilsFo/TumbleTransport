using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonTruck : MonoBehaviour
{

    public GameState state;
    
    private void OnMouseDown()
    {
        state.truckSpawner.DispatchTruck();
    }
}

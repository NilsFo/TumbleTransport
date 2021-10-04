using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TruckDataObject", menuName = "TruckData/TruckDataObject", order = 1)]
public class TruckScriptableObject : ScriptableObject
{
    public GameObject truck;
    public int weight = 100;

    public int fixNumberOfCargoToSpawn = 1;
    
    public float cost = 100f; //Pauschl für Sub Benzien + Driver
    public string nameInScore = "Truck + Driver + Fuel";
}

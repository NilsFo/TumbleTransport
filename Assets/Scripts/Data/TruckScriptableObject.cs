using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TruckDataObject", menuName = "TruckData/TruckDataObject", order = 1)]
public class TruckScriptableObject : ScriptableObject
{
    public int weight = 100;
    public GameObject truck;
}

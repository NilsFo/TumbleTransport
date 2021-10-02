using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TruckSpawnPoolDataObject", menuName = "TruckData/SpawnPoolDataObject", order = 1)]
public class TruckSpawnPoolScriptableObject : ScriptableObject
{
    public TruckScriptableObject[] pool;
    
    public int totalWeight = 0;
    
    public void OnValidate() {
        var tempWeight = 0;
        for (int i = 0; i < pool.Length; i++)
        {
            var myTruck = pool[i];
            if (myTruck != null)
            {
                tempWeight += myTruck.weight;
            }
        }
        totalWeight = tempWeight;
    }
}

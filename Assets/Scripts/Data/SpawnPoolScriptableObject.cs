using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPoolDataObject", menuName = "CargoData/SpawnPoolDataObject", order = 1)]
public class SpawnPoolScriptableObject : ScriptableObject
{
    public CargoScriptableObject[] pool;
    
    public int totalWeight = 0;
    
    public void OnValidate() {
        var tempWeight = 0;
        for (int i = 0; i < pool.Length; i++)
        {
            var myCargo = pool[i];
            if (myCargo != null)
            {
                tempWeight += myCargo.weight;
            }
        }
        totalWeight = tempWeight;
    }
}

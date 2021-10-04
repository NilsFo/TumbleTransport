using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CargoDataObject", menuName = "CargoData/CargoDataObject", order = 1)]
public class CargoScriptableObject : ScriptableObject
{
    public GameObject sprite;
    public int weight = 100;
    
    public bool isUnique = false;
    
    public float purchaseValue = 30f;
    public float salesValue = 90f;

    public string nameInScore = "Cargo";
}

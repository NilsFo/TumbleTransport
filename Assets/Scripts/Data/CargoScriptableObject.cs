using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CargoDataObject", menuName = "CargoData/CargoDataObject", order = 1)]
public class CargoScriptableObject : ScriptableObject
{
    public int weight = 100;
    public GameObject sprite;
}

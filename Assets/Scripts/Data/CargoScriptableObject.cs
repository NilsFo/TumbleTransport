using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CargoDataObject", menuName = "CargoData/CargoDataObject", order = 1)]
public class CargoScriptableObject : ScriptableObject
{
    public enum Tags {Day1, Day2, Day3, Day4, Day5, Fun};
    
    public int weight = 100;
    public Tags tag = Tags.Day1;

    public GameObject sprite;
}

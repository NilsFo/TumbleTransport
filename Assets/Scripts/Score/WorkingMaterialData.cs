using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingMaterialData
{
    public string materialName;
    public float materialCost;
    
    public WorkingMaterialData(float costOfMaterial, string materialOfMaterial)
    {
        materialName = materialOfMaterial;
        materialCost = costOfMaterial;
    }
    
    public WorkingMaterialData(ToolCosts toolCosts)
    {
        materialName = toolCosts.name;
        materialCost = toolCosts.cost;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryData
{
    public List<CargoScriptableObject> fastenedCargo;
    public List<CargoScriptableObject> thrownCargo;

    public TruckScriptableObject truck;
    
    public DeliveryData(TruckScriptableObject pTruck, List<CargoScriptableObject> pFastenedCargo, List<CargoScriptableObject> pThrownCargo)
    {
        fastenedCargo = pFastenedCargo;
        thrownCargo = pThrownCargo;
        truck = pTruck;
    }

    public List<CargoScriptableObject> GetAllCargo()
    {
        List<CargoScriptableObject> allCargo = new List<CargoScriptableObject>();
        allCargo.AddRange(fastenedCargo);
        allCargo.AddRange(thrownCargo);
        return allCargo;
    }

    public float GetPurchaseValueOfCargo()
    {
        float sum = 0f;
        List<CargoScriptableObject> myList = GetAllCargo();
        foreach (var cargo in myList)
        {
            sum += cargo.purchaseValue;
        }
        return sum;
    }
    
    public float GetSalesValueOfCargo()
    {
        float sum = 0f;
        foreach (var cargo in fastenedCargo)
        {
            sum += (cargo.salesValue);
        }
        return sum;
    }

    public float GetSalesGrossProfit()
    {
        float sum = 0f;
        foreach (var cargo in fastenedCargo)
        {
            sum += (cargo.salesValue - cargo.purchaseValue);
        }
        return sum;
    }
    
    public float GetSalesProfit()
    {
        return (GetSalesGrossProfit() - truck.cost);
    }
    
    public int GetNumberOfCargo()
    {
        return fastenedCargo.Count + thrownCargo.Count;
    }
}

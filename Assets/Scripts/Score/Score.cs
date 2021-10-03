using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    public float dailyFixedCosts = 100f;
    public float workersCostPerH = 12.50f;
    public int workersHouers= 5;
    
    public int totalCountTrucks = 0;
    public int totalCountCargo = 0;
    
    public float totalCost = 0;
    public float totalEarnings = 0;

    private List<DeliveryData> listOfDeliveries;
    
    public Score()
    {
        listOfDeliveries = new List<DeliveryData>();
    }

    public void AddDeliveryData(DeliveryData toAdd)
    {
        totalCountTrucks++;
        totalCountCargo += toAdd.GetNumberOfCargo();
        
        totalEarnings += toAdd.GetSalesValueOfCargo();
        
        totalCost += toAdd.GetPurchaseValueOfCargo();
        totalCost += toAdd.truck.cost;
        
        listOfDeliveries.Add(toAdd);
    }

    public float GetProfit()
    {
        return totalEarnings - totalCost - dailyFixedCosts - (workersCostPerH * workersHouers);
    }

    public override string ToString()
    {
        return "" + totalCountTrucks + ", " + totalCountCargo + ", " + totalCost + ", " + totalEarnings;
    }
}

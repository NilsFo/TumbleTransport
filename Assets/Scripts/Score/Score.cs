using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score
{
    public float dailyFixedCosts = 100f;
    public float workersCostPerH = 7.25f;
    public int workersHouers= 8;
    
    public int totalCountTrucks = 0;
    public int totalCountCargo = 0;
    
    public float totalCost = 0;
    public float totalEarnings = 0;
    public float totalProfit = 0;

    private List<DeliveryData> listOfDeliveries;
    private List<WorkingMaterialData> listOfWorkingMaterialsUsed;

    public List<BubbleData> listOfBubbles;
    
    public Score()
    {
        listOfDeliveries = new List<DeliveryData>();
        listOfWorkingMaterialsUsed = new List<WorkingMaterialData>();

        listOfBubbles = new List<BubbleData>();
    }

    public void AddDeliveryData(DeliveryData toAdd)
    {
        totalCountTrucks++;
        totalCountCargo += toAdd.fastenedCargo.Count;
        
        totalEarnings += toAdd.GetSalesValueOfCargo();
        
        totalProfit += toAdd.GetSalesProfit();
        
        totalCost += toAdd.GetPurchaseValueOfCargo();
        totalCost += toAdd.truck.cost;
        
        listOfDeliveries.Add(toAdd);

        //Truck Cost
        SpriteRenderer truckRenderer  = toAdd.truck.truck.transform.GetChild(0).GetComponent<SpriteRenderer>();
        listOfBubbles.Add(new BubbleData(truckRenderer.sprite, toAdd.truck.cost, toAdd.truck.nameInScore, -1));
        
        //Cargo Yield
        for (int i = 0; i < toAdd.fastenedCargo.Count; i++)
        {
            CargoScriptableObject dataObject = toAdd.fastenedCargo[i];
            SpriteRenderer cargoRenderer  = dataObject.sprite.GetComponent<SpriteRenderer>();
            float value = (dataObject.salesValue - dataObject.purchaseValue);
            listOfBubbles.Add(new BubbleData(cargoRenderer.sprite, value, dataObject.nameInScore, 1));
        }
        
        //Cargo Cost
        for (int i = 0; i < toAdd.thrownCargo.Count; i++)
        {
            CargoScriptableObject dataObject = toAdd.thrownCargo[i];
            SpriteRenderer cargoRenderer  = dataObject.sprite.GetComponent<SpriteRenderer>();
            listOfBubbles.Add(new BubbleData(cargoRenderer.sprite, dataObject.purchaseValue, dataObject.nameInScore + " (Dropped)", -1));
        }
    }

    public void AddWorkingMaterialData(WorkingMaterialData data, Sprite toolSprite)
    {
        totalCost += data.materialCost;
        totalProfit -= data.materialCost;
        
        listOfWorkingMaterialsUsed.Add(data);

        listOfBubbles.Add(new BubbleData(toolSprite, data.materialCost, data.materialName, -1));
    }
    
    public float GetProfit()
    {
        return totalEarnings - totalCost - dailyFixedCosts - GetPersonalCost();
    }

    public float GetPersonalCost()
    {
        return GetSalary() * 1.3f;
    }

    public float GetSalary()
    {
        return workersCostPerH * workersHouers;
    }

    public override string ToString()
    {
        return "" + totalCountTrucks + ", " + totalCountCargo + ", " + totalCost + ", " + totalEarnings;
    }
}

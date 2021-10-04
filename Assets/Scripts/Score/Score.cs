using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        totalCountCargo += toAdd.GetNumberOfCargo();
        totalEarnings += toAdd.GetSalesValueOfCargo();
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
            listOfBubbles.Add(new BubbleData(cargoRenderer.sprite, dataObject.purchaseValue, dataObject.nameInScore, -1));
        }
    }

    public void AddWorkingMaterialData(WorkingMaterialData data, Sprite toolSprite)
    {
        totalCost += data.materialCost;
        listOfWorkingMaterialsUsed.Add(data);

        listOfBubbles.Add(new BubbleData(toolSprite, data.materialCost, data.materialName, -1));
    }
    
    public float GetProfit()
    {
        return totalEarnings - totalCost - dailyFixedCosts - (workersCostPerH * workersHouers);
    }

    public float GetPersonalCost()
    {
        return workersCostPerH * workersHouers;
    }

    public float GetSalary()
    {
        return (float)(GetPersonalCost() * 0.7);
    }

    public override string ToString()
    {
        return "" + totalCountTrucks + ", " + totalCountCargo + ", " + totalCost + ", " + totalEarnings;
    }
}

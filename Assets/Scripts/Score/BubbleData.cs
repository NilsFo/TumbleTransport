using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleData
{
    private Sprite image;
    private float value;
    private string name;
    private int vorzeichen = 0; //1 = positiv, 0 = neutral, -1 = negative
    
    public BubbleData(Sprite image, float value, string name, int vorzeichen)
    {
        this.image = image;
        this.value = value;
        this.name = name;
        this.vorzeichen = vorzeichen;
    }

    public float GetValue()
    {
        return value;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public string GetName()
    {
        return name;
    }

    public int GetVorzeichen()
    {
        return vorzeichen;
    }
}

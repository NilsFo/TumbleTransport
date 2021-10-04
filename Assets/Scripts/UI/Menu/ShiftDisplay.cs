using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShiftDisplay : MonoBehaviour
{
    public TextMeshProUGUI textFild;

    private void Start()
    {
        string name = "Early Shift";
        if (GameState.shift == 1)
        {
            name = "Day Shift";
        } 
        else if (GameState.shift == 2)
        {
            name = "Night Shift";
        }
        textFild.text = name;
    }
}

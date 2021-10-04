using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LastScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    
    public Color positive = Color.green;
    public Color negative = Color.red;
    public Color neutral = Color.white;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameState.lastProfit > 0)
        {
            float value = GameState.lastProfit;
            string vorzeichen = "+";
            Color textcolor = positive;
            if (value == 0)
            {
                vorzeichen = "";
                textcolor = neutral;
            }
            else if (value < 0)
            {
                vorzeichen = "-";
                textcolor = negative;
            }
            value = Mathf.Abs(value);
            textMeshProUGUI.text =   vorzeichen + " $" + String.Format("{0:0.00}", Mathf.Round(value * 100f) / 100f);
            textMeshProUGUI.color = textcolor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

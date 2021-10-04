using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestScoreDisplay : MonoBehaviour
{
    public GameObject obj;
    public TextMeshProUGUI textMeshProUGUI;

    public int shift = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (shift == 1)
        {
            if (GameState.bestProfitShift1 > 0)
            {
                obj.SetActive(true);
                textMeshProUGUI.text =  "+ $" + String.Format("{0:0.00}", Mathf.Round(GameState.bestProfitShift1 * 100f) / 100f);
            }
            else
            {
                obj.SetActive(false);
            }
        }
        else if (shift == 1)
        {
            if (GameState.bestProfitShift2 > 0)
            {
                obj.SetActive(true);
                textMeshProUGUI.text =  "+ $" + String.Format("{0:0.00}", Mathf.Round(GameState.bestProfitShift2 * 100f) / 100f);
            }
            else
            {
                obj.SetActive(false);
            }
        }
        else
        {
            if (GameState.bestProfitShift0 > 0)
            {
                obj.SetActive(true);
                textMeshProUGUI.text =  "+ $" + String.Format("{0:0.00}", Mathf.Round(GameState.bestProfitShift0 * 100f) / 100f);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }

    
}

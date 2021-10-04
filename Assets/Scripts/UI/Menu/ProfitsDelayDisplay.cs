using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfitsDelayDisplay : MonoBehaviour
{
    public GameObject obj;
    public TextMeshProUGUI label;

    public Color positive = Color.green;
    public Color negative = Color.red;
    public Color neutral = Color.white;
    
    public float delayToDisplay = 1f;
    private float _currentDelayTime = 0f;

    public AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        obj.SetActive(false);
        _currentDelayTime = 0f;
        
        float value = GameState.score.GetProfit();
        GameState.lastProfit = value;
        GameState.lastShift = GameState.shift;
        
        if (GameState.shift == 1)
        {
            if (value > GameState.bestProfitShift1)
            {
                GameState.bestProfitShift1 = value;
            }
        }
        else if(GameState.shift == 2)
        {
            if (value > GameState.bestProfitShift2)
            {
                GameState.bestProfitShift2 = value;
            }
        }
        else
        {
            if (value > GameState.bestProfitShift0)
            {
                GameState.bestProfitShift0 = value;
            }
        }
        
        GameState.saveData();
        
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
        label.text = vorzeichen + " $" + String.Format("{0:0.00}", Mathf.Round(value * 100f) / 100f);
        label.color = textcolor;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentDelayTime > delayToDisplay)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true); 
                audioSource.Play();
            }
        }
        else
        {
            _currentDelayTime += Time.deltaTime;
        }
    }
}

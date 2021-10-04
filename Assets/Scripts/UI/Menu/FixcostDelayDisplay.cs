using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixcostDelayDisplay : MonoBehaviour
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
        float value = GameState.score.dailyFixedCosts;
        string vorzeichen = "-";
        Color textcolor = negative;
        if (value == 0)
        {
            vorzeichen = "";
            textcolor = neutral;
        }
        else if (value < 0)
        {
            value = Mathf.Abs(value);
            vorzeichen = "+";
            textcolor = positive;
        }
        
        label.text = vorzeichen + " $" + value;
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

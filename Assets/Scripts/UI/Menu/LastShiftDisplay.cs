using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastShiftDisplay : MonoBehaviour
{
    public Button button;
    
    public int shift = 0;
    
    public Color highlight = Color.black;
    public Color normal = Color.gray; 
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameState.shift == shift)
        {
            var colors = button.colors;
            colors.normalColor = highlight;
            button.colors = colors;
        }
        else
        {
            var colors = button.colors;
            colors.normalColor = normal;
            button.colors = colors;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

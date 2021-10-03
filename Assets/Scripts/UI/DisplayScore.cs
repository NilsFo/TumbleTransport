using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = "Score: " + GameState.score.GetProfit();
    }
}

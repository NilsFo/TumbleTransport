using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CargoCounterDisplay : MonoBehaviour
{
    public GameState gameState;
    public TextMeshProUGUI textMeshProUGUI;

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = "" + GameState.score.totalCountCargo;
    }
}

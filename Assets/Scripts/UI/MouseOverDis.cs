using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseOverDis : MonoBehaviour
{
    public TextMeshProUGUI textFild;
    public string text;

    private void OnMouseOver()
    {
        textFild.text = text;
    }
}

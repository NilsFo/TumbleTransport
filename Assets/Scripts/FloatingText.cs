using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Color textColor=Color.white;
    
    public TMPro.TextMeshProUGUI textMesh;
    public float fontSize = 5;
    public string text;
    public Vector3 velocity;
    public float duration;
    private float durationCurrent;

    // Start is called before the first frame update
    void Start()
    {
        if (text == null) text = " ";
        
        durationCurrent = duration;
        textMesh.text = text;
        textMesh.color = textColor;
        textMesh.fontSize = fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        durationCurrent -= Time.deltaTime;

        Vector3 v = velocity * Time.deltaTime;
        Vector3 newPos = transform.position;

        newPos.x = newPos.x + v.x;
        newPos.y = newPos.y + v.y;
        newPos.z = newPos.z + v.z;

        transform.position = newPos;
        if (durationCurrent < 0)
        {
            Destroy(gameObject);
        }
    }
}
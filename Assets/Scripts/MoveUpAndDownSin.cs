using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDownSin : MonoBehaviour
{

    private float existsTime;
    private float startY;

    public float animationSpeed = 100;

    // Start is called before the first frame update
    void Start()
    {
        existsTime = 0;
        startY=transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        existsTime = existsTime += Time.deltaTime;

        var p = transform.position;
        p.y = startY + (Mathf.Sin(existsTime * animationSpeed)/2);
        transform.position = p;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EinfligAI : MonoBehaviour
{

    private float originX;
    public float travelDistance = 100;
    public float delay;
    public float speed;
    private float startDelayCurrent;
    
    // Start is called before the first frame update
    void Start()
    {
        startDelayCurrent = delay;
    }

    // Update is called once per frame
    void Update()
    {
        return;
        startDelayCurrent -= Time.deltaTime;

        if (startDelayCurrent<0)
        {
            var pos = transform.localPosition;
            float currentX = pos.x;
            if (originX>currentX)
            {
                pos.x = currentX - speed * Time.deltaTime;
                transform.localPosition = pos;
            }
        }
    }
}

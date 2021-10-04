using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BubbleMoveAndDestroy : MonoBehaviour
{
    public float speed = 0.75f;
    public float cutOffY = 6f;

    public TextMeshProUGUI labelText;
    public TextMeshProUGUI costText;
    
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + (speed * Time.deltaTime),
            transform.position.z);
        
        if (transform.position.y > cutOffY)
        {
            Destroy(gameObject);
        }
    }
}

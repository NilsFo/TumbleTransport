using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LashingStrap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //SetLashingStrap(new Vector3(-1.49000001f,-0.479999989f,-6f), new Vector3(1.45000005f,0.649999976f,-6f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLashingStrap(Vector3 start, Vector3 end)
    {
        float ourZ = Math.Min(start.z, end.z);
        start.z = ourZ;
        end.z = ourZ;
        var pos = (start + end) / 2;
        pos.z = -5;
        transform.position = pos;
        var d = start - end;
        var alpha = Mathf.Rad2Deg * Mathf.Atan2(d.y, d.x);
        transform.rotation = Quaternion.Euler(0, 0, alpha);

        var length = d.magnitude + 0.25f;
        var boxCollider = GetComponent<BoxCollider2D>();
        var bcSize = boxCollider.size;
        bcSize.x = length;
        boxCollider.size = bcSize;

        var sprite = GetComponent<SpriteRenderer>();
        var spSize = sprite.size;
        spSize.x = length;
        sprite.size = spSize;


    }
}

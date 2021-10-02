using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCargo : MonoBehaviour
{

    public GameObject spawnPrefab;

    public BoxCollider2D spawnArea;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var bounds = spawnArea.bounds;
            var xpos = Random.Range(bounds.min.x ,bounds.max.x);
            var ypos = Random.Range(bounds.min.y ,bounds.max.y);
            Instantiate(spawnPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity);
        }
    }
}

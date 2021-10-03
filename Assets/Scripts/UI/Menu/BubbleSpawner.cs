using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
  public GameObject prefabTruckBubble;
  public GameObject prefabMaterialBubble;

  public GameObject spawnPoint;

  public float spawnTime = 1.0f;

  private float _currentSpawnTime = 0;
  
  private void Update()
  {
    if (_currentSpawnTime < spawnTime)
    {
      _currentSpawnTime += Time.deltaTime;
    }
    else
    {
      SpawnBubble();
      _currentSpawnTime = 0;
    }
  }

  private void SpawnBubble()
  {
    GameObject newBubble = Instantiate(prefabTruckBubble, spawnPoint.transform.position, Quaternion.identity, transform);
  }
}

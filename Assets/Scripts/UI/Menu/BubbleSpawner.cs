using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSpawner : MonoBehaviour
{
  public GameObject prefabBubble;

  public GameObject spawnPoint;

  public float spawnTime = 1.0f;

  private float _currentSpawnTime = 0;

  private int pointer = 0;
  
  public Color positive = Color.green;
  public Color negative = Color.red;
  public Color neutral = Color.white;
  
  private void Update()
  {
    if (_currentSpawnTime < spawnTime)
    {
      _currentSpawnTime += Time.deltaTime;
    }
    else
    {
      if (pointer < GameState.score.listOfBubbles.Count)
      {
        BubbleData data = GameState.score.listOfBubbles[pointer]; 
        SpawnBubble(data);
        pointer += 1;
      }
      else
      {
        pointer = 0;
      }
      _currentSpawnTime = 0;
    }
  }

  private void SpawnBubble(BubbleData data)
  {
    GameObject newBubble = Instantiate(prefabBubble, spawnPoint.transform.position, Quaternion.identity, transform);
    Image bubbleImage = newBubble.GetComponent<Image>();
    if (bubbleImage != null)
    {
      bubbleImage.sprite = data.GetImage();
    }
    BubbleMoveAndDestroy bubbleMoveAndDestroy = newBubble.GetComponent<BubbleMoveAndDestroy>();
    if (bubbleMoveAndDestroy != null)
    {
      bubbleMoveAndDestroy.labelText.text = data.GetName();

      float value = data.GetValue();
      value = Mathf.Abs(value);
      string vorzeichen = "+";
      Color textcolor = positive;
      if (data.GetVorzeichen() == 0)
      {
        vorzeichen = "";
        textcolor = neutral;
      }
      else if (data.GetVorzeichen() < 0)
      {
        vorzeichen = "-";
        textcolor = negative;
      }
      
      bubbleMoveAndDestroy.costText.text =  vorzeichen + " $" + value;
      bubbleMoveAndDestroy.costText.color = textcolor;
    }
  }
}

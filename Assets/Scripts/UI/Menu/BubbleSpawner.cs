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
    BubbleMoveAndDestroy bubbleMoveAndDestroy = newBubble.GetComponentInChildren<BubbleMoveAndDestroy>();
    if (bubbleMoveAndDestroy != null)
    {
      bubbleMoveAndDestroy.labelText.text = data.GetName();
      bubbleMoveAndDestroy.image.sprite = data.GetImage();

      //Fix Scaling modified SetNativeSize => bubbleMoveAndDestroy.image.SetNativeSize();
      RectTransform imageRectTransform = bubbleMoveAndDestroy.image.gameObject.GetComponent<RectTransform>();
      if (imageRectTransform != null)
      {
        float w = bubbleMoveAndDestroy.image.sprite.rect.width / bubbleMoveAndDestroy.image.pixelsPerUnit;
        float h = bubbleMoveAndDestroy.image.sprite.rect.height / bubbleMoveAndDestroy.image.pixelsPerUnit;
        imageRectTransform.anchorMax = imageRectTransform.anchorMin;
        if (h > 190)
        {
          h = 190;
          w = 110;
        }
        imageRectTransform.sizeDelta = new Vector2(w, h);
      }

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
      
      bubbleMoveAndDestroy.costText.text =  vorzeichen + " $" + String.Format("{0:0.00}", Mathf.Round(value * 100f) / 100f);
      bubbleMoveAndDestroy.costText.color = textcolor;
    }
  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SecretSpawner : MonoBehaviour
{
    public float delayToDisplay = 1f;
    private float _currentDelayTime = 0f;

    public AudioSource audioSource;
    
    public GameObject FiredObj;
    public GameObject CounterObj;
    public TextMeshProUGUI Counter;
    
    private int countSpawner = 0;

    // Start is called before the first frame update
    void Start()
    {
        _currentDelayTime = 0f;
        float salary = GameState.score.GetSalary();
        if (GameState.score.GetProfit() < (-1 * salary))
        {
            countSpawner = Math.Abs((int)((GameState.score.GetProfit() - salary) / salary))-1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentDelayTime > delayToDisplay && countSpawner > 1)
        {
            if (!FiredObj.activeSelf)
            {
                FiredObj.SetActive(true);
                if (countSpawner > 2)
                {
                    CounterObj.SetActive(true);
                    Counter.text = "X"+countSpawner;
                }
                if (audioSource != null)
                {
                    audioSource.Play();
                }

                GameState.firedCounter = countSpawner;
            }
        }
        else
        {
            _currentDelayTime += Time.deltaTime;
        }
    }
}

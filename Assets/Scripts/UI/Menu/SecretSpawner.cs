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
        FiredObj.SetActive(false);
        CounterObj.SetActive(false);
        _currentDelayTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentDelayTime > delayToDisplay)
        {
            float salary = GameState.score.GetSalary();
            if (GameState.lastShift != -1 && GameState.lastProfit < (-1 * salary))
            {
                countSpawner = Math.Abs((int)((GameState.lastProfit - salary) / salary))- 1;
            }
            
            if (!FiredObj.activeSelf && countSpawner > 0)
            {
                FiredObj.SetActive(true);
                if (countSpawner > 2)
                {
                    CounterObj.SetActive(true);
                    Counter.text = "(x"+countSpawner+")";
                }
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
        else
        {
            _currentDelayTime += Time.deltaTime;
        }
    }
}

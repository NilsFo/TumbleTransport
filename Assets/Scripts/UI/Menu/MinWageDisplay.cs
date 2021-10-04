using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinWageDisplay : MonoBehaviour
{
    public GameObject obj;
    public TextMeshProUGUI label;

    public float delayToDisplay = 1f;
    private float _currentDelayTime = 0f;

    public AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        obj.SetActive(false);
        _currentDelayTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentDelayTime > delayToDisplay)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true); 
                audioSource.Play();
            }
        }
        else
        {
            _currentDelayTime += Time.deltaTime;
        }
    }
}

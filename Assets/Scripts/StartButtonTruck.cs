using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartButtonTruck : MonoBehaviour
{

    public GameState state;
    public TMPro.TMP_Text myText;
    public float animationSpeed = 0.69f;
    public float textColorSpeed = 25f;

    public Color defaultColor;
    public Color blinkColor;

    private float aliveTime = 0;
    private string originalText;
    private float animationProgress;
    private int animationWordProgress;
    public bool animationPlaying;
    
    private void Start()
    {
        originalText = myText.text;
        animationProgress = 0;
        animationWordProgress = 0;
        animationPlaying = false;
    }

    private void Update()
    {
        myText.text = originalText;
        aliveTime = aliveTime + Time.deltaTime;
        myText.color = defaultColor;

        if (animationPlaying)
        {
            myText.enabled = true;
            animationProgress += Time.deltaTime;
            if (animationProgress > animationSpeed)
            {
                animationProgress = 0;
                animationWordProgress = animationWordProgress + 1;
            }
            myText.text = originalText.Substring(0, Math.Min(animationWordProgress,originalText.Length));

            if (Mathf.Sin(aliveTime*textColorSpeed) < 0)
            {
                myText.color = blinkColor;
            }

            // if (animationWordProgress==originalText.Length)
            // {
            //     animationPlaying = false;
            // }
        }
    }

    private void OnMouseDown()
    {
        if (myText.enabled)
        {
            state.truckSpawner.DispatchTruck();
        }
    }

    public void AnimateButtonText()
    {
        if (myText.enabled)
        {
            return;
        }
        animationPlaying = true;
    }
    
}

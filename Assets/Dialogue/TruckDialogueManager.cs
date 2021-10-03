using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TruckDialogueManager : MonoBehaviour
{
    public Transform radioPos;
    public GameState gameState;
    
    private TextBubbleManager textBubbleManager;

    private float busyTimer = 0;

    public String[] truckerWaitQuotes = {
        "Let's go, I've got places to be",
        "I'll just leave the motor running",
        "What's the hold up?",
        "Is there a problem?",
        "Get on with it",
        "I'm waiting!",
        "This stuff has to get out there ASAP!",
        "What are you doing? My job's on the line here!",
        "Gotta be home for dinner with the missus soon",
        "Do your job so I can do mine",
        "Let's go, let's go!",
        "No time for hanging around",
        "What is this? The daycare?",
        "I've had inspections that were faster than this",
        "Did you fall asleep?",
        "Hello? Load me up!"
    };
    
    // Start is called before the first frame update
    void Start()
    {
        textBubbleManager = GetComponent<TextBubbleManager>();
    }

    public void RadioMessage(string message, float duration) {
        textBubbleManager.ClearDialogueBoxes();
        textBubbleManager.Say(radioPos, message, duration);
    }
    
    // Update is called once per frame
    void Update()
    {
        busyTimer = busyTimer - Time.deltaTime;

    }

    public bool AcceptsAmbientMessages()
    {
        //return gameState.PlayState == GameState.GameplayState.Playing && busyTimer < 0f;
        return true;
    }

    public void RequestRadioMessage(string text, float time)
    {
        if (AcceptsAmbientMessages())
        {
            busyTimer = time;
            RadioMessage(text, time);
        }
    }

    public void ReadRandomWaitingQuote() {
        if (busyTimer > 0)
            return;
        var i = Random.Range(0, truckerWaitQuotes.Length);
        var quote = truckerWaitQuotes [i];
        textBubbleManager.ClearDialogueBoxes();
        textBubbleManager.Say(radioPos, quote);
    }
    
}

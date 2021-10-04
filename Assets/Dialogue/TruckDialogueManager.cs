using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TruckDialogueManager : MonoBehaviour
{
    public Transform radioPos;
    public GameState gameState;
    
    public TextBubbleManager textBubbleManager;

    private float busyTimer = 0;

    public String[] truckerWaitQuotes = {
        "Let's go, I've got places to be!",
        "I'll just leave the motor running.",
        "What's the hold up?",
        "Is there a problem?",
        "Get on with it!",
        "I'm waiting!",
        "This stuff has to get out there ASAP!",
        "What are you doing? My job's on the line here!",
        "Gotta be home for dinner with the missus soon!",
        "Do your job so I can do mine!",
        "Let's go, let's go!",
        "No time for hanging around!",
        "What is this? The daycare?",
        "I've had inspections that were faster than this!",
        "Did you fall asleep?",
        "I'm already on overtime, so make this quick.",
        "Hurry up, slowpoke!",
        "What's keeping you so long... geez?",
        "Sorry, I don't mean to be mean. It's just the booze talking...",
        "Hurry up, or I'll speak with your foreman!",
        "Want me to come up there and load this thing by myself?",
        "Hello? Load me up!"
    };
    
    
    // Start is called before the first frame update
    void Start()
    {
        textBubbleManager = GetComponent<TextBubbleManager>();
        gameState = FindObjectOfType<GameState>();
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
        var quote = GetNextDriverQuote();
        textBubbleManager.ClearDialogueBoxes();
        textBubbleManager.Say(radioPos, quote);
    }

    public String GetNextDriverQuote()
    {
        var i = Random.Range(0, truckerWaitQuotes.Length);
        var quote = truckerWaitQuotes [i];

        if (!gameState.tutorialHasDepartedAtLeastOnce)
        {
            quote = "Use the 'Dispatch' button when you're ready.";
        }
        if (!gameState.tutorialHasTooledAtLeastOnce)
        {
            quote = "Use the tools on the conveyor belt to fasten the cargo.";
        }
        if (!gameState.tutorialHasLoadedTruckAtLeastOnce)
        {
            quote = "Use the mouse to pick up and place some cargo on the truck. Right click to rotate.";
        }
        return quote;
    }
    
}

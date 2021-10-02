using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    public Transform radioPos;
    public GameState gameState;
    
    private TextBubbleManager textBubbleManager;

    private float busyTimer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        textBubbleManager = GetComponent<TextBubbleManager>();
        Invoke("DialogueLine1", 1);
        //Invoke("DialogueLine2", 3+5);
        //Invoke("DialogueLine3", 3+5+7);
    }

    public void RadioMessage(string message, float duration) {
        textBubbleManager.ClearDialogueBoxes();
        textBubbleManager.Say(transform, message, duration);
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

    public void DialogueLine1() {
        textBubbleManager.Say(transform, "Get stacking son! I don't pay you for standing around!", 5);
        busyTimer = 8;
    }
    
}

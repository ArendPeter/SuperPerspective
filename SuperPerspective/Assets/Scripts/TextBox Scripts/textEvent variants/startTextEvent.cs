using UnityEngine;
using System.Collections;

public class startTextEvent : textEvent {

    //Event that starts a conversation in the textBoxScript.

    public textBoxScript tb;
    public convoNode myConversation;

    void Start()
    {
        if (tb == null)
        {
            GameObject.FindObjectOfType<textBoxScript>();
        }
    }

	public void eventTrigger()
    {
        tb.startConvo(myConversation);
    }
}

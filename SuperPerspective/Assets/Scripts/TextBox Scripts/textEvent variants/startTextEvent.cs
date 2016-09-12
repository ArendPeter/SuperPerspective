using UnityEngine;
using System.Collections;

public class startTextEvent : textEvent
{

    //Event that starts a conversation in the textBoxScript.

    private textBoxScript tb;
    public convoNode myConversation;

    void Start()
    {
        if (!(tb != null))
        {
           tb = GameObject.FindObjectOfType<textBoxScript>();
        }
    }

	public override void eventTrigger()
    {
        
        tb.startConvo(myConversation);
    }
}

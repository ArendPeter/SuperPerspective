using UnityEngine;
using System.Collections;

public class startTextEvent : textEvent
{

    //Event that starts a conversation in the textBoxScript.

    
    [SerializeField] private textBoxScript tb;
    [SerializeField] private askBoxScript ab;
    public convoNode myConversation;

    void Start()
    {
        if (!(tb != null))
        {
           tb = GameObject.FindObjectOfType<textBoxScript>();
        }

        if(!(ab != null))
        {
            ab = GameObject.FindObjectOfType<askBoxScript>();
        }
    }

	public override void eventTrigger()
    {
        tb.disableBox();
        //ab.textBoxPartner.startConvo(myConversation);
        ab.disableBox();
        ab.showBox = false;
        tb.startConvo(myConversation);
    }
}

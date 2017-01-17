using UnityEngine;
using System.Collections;

public class startTextEvent : textEvent
{

    //Event that starts a conversation in the textBoxScript.

    
    private textBoxScript tb;
    private askBoxScript ab;
    public convoNode myConversation;
    public bool startCrystalFairyTheme;
    private UISFXManager sfx;

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

        if(!(sfx != null))
        {
            sfx = GameObject.FindObjectOfType<UISFXManager>();
        }
    }

	public override void eventTrigger()
    {
        tb.disableBox();
        //ab.textBoxPartner.startConvo(myConversation);
        ab.disableBox();
        ab.showBox = false;
        tb.startConvo(myConversation);

        if (startCrystalFairyTheme)
        {
            sfx.PlayCrystalFairyTheme();
        }

    }
}

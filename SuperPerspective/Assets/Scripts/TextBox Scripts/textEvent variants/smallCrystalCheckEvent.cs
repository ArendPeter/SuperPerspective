using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smallCrystalCheckEvent : textEvent {

    //Event that starts a conversation in the textBoxScript.


    private textBoxScript tb;
    private askBoxScript ab;
    public bool startCrystalFairyTheme;
    private UISFXManager sfx;

    public GreatCrystalWall wall;

    public convoNode successConvo;
    public convoNode failureConvo;

    void Start()
    {
        if (!(tb != null))
        {
            tb = GameObject.FindObjectOfType<textBoxScript>();
        }

        if (!(ab != null))
        {
            ab = GameObject.FindObjectOfType<askBoxScript>();
        }

        if (!(sfx != null))
        {
            sfx = GameObject.FindObjectOfType<UISFXManager>();
        }
    }

    public override void eventTrigger()
    {
        tb.disableBox();
        ab.disableBox();
        ab.showBox = false;

        if (wall.gotAllSmallCrystals)
        {
            tb.startConvo(successConvo);
        }
        else
        {
            tb.startConvo(failureConvo);
        }

        if (startCrystalFairyTheme)
        {
            sfx.PlayCrystalFairyTheme();
        }

    }
}

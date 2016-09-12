using UnityEngine;
using System.Collections;

public class crystalExplosionConversationEndedEvent : textEvent {

    //Event that starts a conversation in the textBoxScript.

    public CrystalExplosionTrigger cTrig;

    [Tooltip("The ConversationEnded bool in the Crystal Explosion Trigger should be set to this.")]
    public bool ConversationBoolShouldBeSetTo = true;

    void Start()
    {
        if (!(cTrig != null))
        {
            cTrig = GameObject.FindObjectOfType<CrystalExplosionTrigger>();
        }
    }

    public override void eventTrigger()
    {

        cTrig.conversationEnded = ConversationBoolShouldBeSetTo;
    }
}

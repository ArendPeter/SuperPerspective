using UnityEngine;
using System.Collections;

public class ivanOpeningEvent : textEvent{

    public FairyFollow ff;
    private InteractableSpirit spirit;

    public Collider blocker;

    public bool setFollowTo = true;

    public bool stopCrystalFairyTheme;
    private UISFXManager sfx;

    //keeps track of notification marker
    static NotificationController notiMarker;
    protected GameObject player;

    // Use this for initialization
    void Start () {
        player = PlayerController.instance.gameObject;
        //ff = GameObject.FindObjectOfType<FairyFollow>();
        spirit = ff.gameObject.GetComponent<InteractableSpirit>();
        notiMarker = player.transform.Find("Notification").GetComponent<NotificationController>();

        if (!(sfx != null))
        {
            sfx = GameObject.FindObjectOfType<UISFXManager>();
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public override void eventTrigger()
    {
        //print("TRIGGER!");
        this.setNotMarkerVisibility(false);
        setFairyFollow(setFollowTo);
        deleteSpirit();
        deleteBarrier();

        if (stopCrystalFairyTheme)
        {
            sfx.StopCrystalFairyTheme();
        }

    }

    public void setNotMarkerVisibility(bool set)
    {
        notiMarker.updateVisible(set);
    }

    public void setFairyFollow(bool set)
    {
        if(!(ff != null))
        {
            ff = GameObject.FindObjectOfType<FairyFollow>();
        }
        ff.shouldFollow = set;
        ff.findPlayer();
    }

    public void deleteSpirit()
    {
        spirit.gameObject.GetComponent<Collider>().enabled = false;
        spirit.enabled = false;
    }

    public void deleteBarrier()
    {
        blocker.enabled = false;
    }

}

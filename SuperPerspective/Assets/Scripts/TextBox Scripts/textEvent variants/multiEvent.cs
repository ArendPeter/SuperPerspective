using UnityEngine;
using System.Collections;

public class multiEvent : textEvent {

    public textEvent[] eventArray;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void eventTrigger()
    {
        for(int i = 0; i < eventArray.Length; i++)
        {
            
            if (eventArray[i] != null)
            {
                print("lol");
                eventArray[i].eventTrigger();
            }
}
    }
}

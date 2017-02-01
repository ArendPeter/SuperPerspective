using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatCrystalWall : MonoBehaviour {

    public Activatable[] switches;
    bool grass = false, ice = false, desert = false, activated = false;

	// Use this for initialization
	void Start () {
        
	}

    public void Activate()
    {
        if (!activated/*&&grass && ice && desert*/)
        {
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i].activated = true;
                activated = true;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

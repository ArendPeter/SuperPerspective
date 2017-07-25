using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desert8_ActiveDynamicCameraToggler : MonoBehaviour {

    public ActiveDynamicCamera cam1, cam2;
    public ToggleSwitch tSwitch;

    //Only enables these cameras for when platform has been risen for the crystal puzzle

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (tSwitch.IsToggled() != cam1.panOnActivate)
        {
            cam1.panOnActivate = tSwitch.IsToggled();
            cam2.panOnActivate = tSwitch.IsToggled();
        }
	}
}

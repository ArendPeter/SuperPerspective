﻿using UnityEngine;
using System.Collections;

public class ToggleSwitch : ActiveInteractable {

	//suppress warnings
	#pragma warning disable 414

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool toggleEnabled = false; //whether switch is currently toggleEnabled

	float distThresh = 1.5f; //distance threshhold where it will become unpressed

	void Start() {
		base.StartSetup ();
		range = 1.5f;
	}

	public override float GetDistance() {
		if (GameStateManager.is3D())
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
			                        new Vector2(transform.position.x, transform.position.y));
	}

	public override void Triggered(){
		Toggle();//toggle switch
	}

	void Toggle(){
		PlayToggleSound();

		toggleEnabled = !toggleEnabled;//enable toggles
		//toggleEnabled is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(toggleEnabled);
	}

	private void PlayToggleSound(){
		//Nick Stuff
		if (!toggleEnabled) {
			gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Objects/Switch/SwitchOn")  as AudioClip;
		}

		else {
			gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Objects/Switch/SwitchOff")  as AudioClip;
		}

		gameObject.GetComponent<AudioSource>().Play ();
	}

    public bool IsToggled()
    {
        return toggleEnabled;
    }
}

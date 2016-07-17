using UnityEngine;
using System.Collections;

public class Bonfire : ActiveInteractable {

	//suppress warnings
	#pragma warning disable 414

	bool toggleEnabled = false; //whether switch is currently toggleEnabled

	float distThresh = 1.5f; //distance threshhold where it will become unpressed

	public Bonfire_UI bonfireUI;

	bool active = false;

	string[] foundDoors;

	void Start() {
		base.StartSetup ();
		range = 1.5f;
	}

	void Update() {
		if (active) {
			if (bonfireUI.teleportFlag) {
				if (foundDoors[bonfireUI.choice] != "")
					Door.TeleportPlayerToDoor(player.GetComponent<PlayerController>(), foundDoors[bonfireUI.choice]);
				bonfireUI.exit();
				bonfireUI.teleportFlag = false;
				active = true;
			}
		}
	}

	public override void Triggered(){
		Toggle();
	}

	void Toggle(){
		string level = Application.loadedLevelName;
		foundDoors = PlayerPrefs.GetString(level).Split(',');
		bonfireUI.maxIsle = foundDoors.Length - 1;
		if (!active) {
			bonfireUI.ToggleOn();
			active = true;
		}
	}

}

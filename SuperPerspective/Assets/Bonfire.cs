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

	public override float GetDistance() {
		if (GameStateManager.is3D())
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
				new Vector2(transform.position.x, transform.position.y));
	}

	void Update() {
		if (active) {
			if (bonfireUI.teleportFlag) {
				if (foundDoors[bonfireUI.choice] != "")
					Door.TeleportPlayerToDoor(player.GetComponent<PlayerController>(), foundDoors[bonfireUI.choice]);
				bonfireUI.ToggleOff();
				bonfireUI.teleportFlag = false;
				active = false;
			}
		}
	}

	public override void Triggered(){
		Toggle();
	}

	void Toggle(){
		string level = Application.loadedLevelName;
		foundDoors = PlayerPrefs.GetString(level).Split(',');
		bonfireUI.maxChoice = foundDoors.Length;
		if (!active) {
			bonfireUI.ToggleOn();
			active = true;
		}
	}

}

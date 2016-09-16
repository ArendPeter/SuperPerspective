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
		range = 2f;
        bonfireUI = GameObject.FindObjectOfType<Bonfire_UI>();
	}

	public override float GetDistance() {
		if (GameStateManager.is3D())
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Mathf.Abs(transform.position.x - player.transform.position.x);
	}

	void Update() {
		if (active) {
			if (bonfireUI.teleportFlag || bonfireUI.closeFlag) {
				if (bonfireUI.teleportFlag && foundDoors[bonfireUI.choice] != "")
					Door.TeleportPlayerToDoor(player.GetComponent<PlayerController>(), foundDoors[bonfireUI.choice]);
				bonfireUI.exit();
				bonfireUI.teleportFlag = false;
				active = false;
				GameStateManager.instance.ExitWaystoneState();
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
			GameStateManager.instance.EnterWaystoneState();
		}
	}

}

﻿using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	//public Door dest;
	public string myName;
	public string destName;
	Door destDoor;
	//public Color particleColor;

	public string sceneName;
	public int crystalRequirement;

	public Vector3 teleportOffset = new Vector3(0,3,0);
    public GameObject warpSound;

    private AdviceGiver AG;

    ListenerHandler l;

	public void Awake(){
		//update particle color
		l = GameObject.Find("AudioListener").GetComponent<ListenerHandler>();


		/*ParticleSystem p = this.transform.FindChild("Particles").GetComponent<ParticleSystem>();
		if(p != null){
			p.startColor = particleColor;
			p.Simulate(2f);
			p.Play();
		}*/

        range = 3;
	}

	public static void TeleportPlayerToDoor(PlayerController p, string doorName) {
		Door dest = DoorManager.instance.getDoor(doorName);
		p.Teleport(
			dest.GetComponent<Collider>().bounds.center + dest.teleportOffset);
		Instantiate(dest.warpSound);
	}

	public override float GetDistance() {
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
			                        new Vector2(transform.position.x, transform.position.y));
	}

	protected override bool isPlayerFacingObject() {
		Vector3 pPos = player.transform.position;
		Bounds bounds = GetComponent<Collider>().bounds;
		if (pPos.x >= bounds.min.x && pPos.x <= bounds.max.x && pPos.z >= bounds.min.z && pPos.z <= bounds.max.z) {
			return true;
		}
		return base.isPlayerFacingObject();
	}

	public override void Triggered(){
		//Creates game object that plays warp sound then self destructs -Nick
        Instantiate(warpSound);

		if(sceneName != "") {
			ResetController.LoadScene(sceneName);
			if (destName != "") {
				TransitionManager.instance.MovePlayerToDoor(player.GetComponent<PlayerController>(), destName);
			}
		}else if(destDoor!=null && MainCollectable.GetMainCollectableHeld() >= crystalRequirement){

            if (AG == null)
            {
                AG = GameObject.FindObjectOfType<AdviceGiver>();
            }
            print("Teleport to " + destName);
            AG.currentLoc = destName;

            player.GetComponent<PlayerController>().Teleport(
				destDoor.GetComponent<Collider>().bounds.center + teleportOffset);

			// Saving level progress
			string level = Application.loadedLevelName;
			string doorsFound = PlayerPrefs.GetString(level);
			if (destName.Contains("start")) {
				if (doorsFound.Equals("")) {
					doorsFound += destDoor.getName();
					PlayerPrefs.SetString(level, doorsFound);
					PlayerPrefs.Save();
				} else if (!doorsFound.Contains(destDoor.getName())) {
					doorsFound += "," + destDoor.getName();
					PlayerPrefs.SetString(level, doorsFound);
					PlayerPrefs.Save();
				}
			}
		}else{
			Debug.Log("Door not linked");
		}
    l.ResetZ();
  }

	public string getName(){
		return myName;
	}

	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}

	protected override bool IsInYRange(){
		float playerY = PlayerController.instance.transform.position.y;
		float myY = transform.position.y;
		float deltaY = playerY - myY;
		return -.5f < deltaY && deltaY < 4f;
	}

}

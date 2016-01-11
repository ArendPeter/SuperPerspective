﻿using UnityEngine;
using System.Collections;

public class PlayerSpawnController : MonoBehaviour {

	public bool startAtDoor = false;
	public string startDoorName;
	Door destDoor;

	void Start () {
		destDoor = this.findDoor(startDoorName);
		if(startAtDoor){
			this.moveToDoor(destDoor);
		}
	}

	//spawn player at door
	public void moveToDoor(Door doorObject){
		if(doorObject != null)
			this.gameObject.GetComponent<PlayerController>().Teleport(
				doorObject.transform.position + new Vector3(0,2,-2));
	}

	//find door with name
	public Door findDoor(string doorName){
		Door[] doorList = Object.FindObjectsOfType(
			typeof(Door)) as Door[];

		foreach(Door door in doorList){
			if(door.getName() == doorName){
				return door;
			}
		}
		//print("PlayerSpawnController: Door ("+doorName+") does not exist");
		return null;
	}

	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}

	public Door getDefaultDest(){
		return destDoor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

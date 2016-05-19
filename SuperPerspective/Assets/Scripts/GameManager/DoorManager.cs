using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorManager : MonoBehaviour {

	#pragma warning disable 168

	public static DoorManager instance;

	Dictionary<string,Door> doors =
		new Dictionary<string,Door>();

	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(instance);
	}

	void Start () {
		//fill up doors
		Door[] doorList = RefreshDoorList();
		//for each door assign destination
		foreach(Door door in doorList){
			Door destDoor;
			doors.TryGetValue(door.destName, out destDoor);
			door.setDoor(destDoor);
		}
	}

	public Door[] RefreshDoorList() {
		Door[] doorList = Object.FindObjectsOfType(
			typeof(Door)) as Door[];
		//fill up doors
		foreach(Door door in doorList){
			try{
				if(door.myName!="")
					doors.Add(door.myName,door);
			}catch(System.Exception e){
				print("DoorManager : There are two doors named "+door.myName+". Please rename one of them");
			}
		}
		return doorList;
	}

	public Door getDoor(string doorName){
		Door returnDoor = null;
		doors.TryGetValue(doorName, out returnDoor);
		return returnDoor;
	}
}

using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour {

	public static TransitionManager instance;

	static bool move = false;
	static string toDoor;

	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(instance);

	}

	void FixedUpdate() {
		if (move) {
			PlayerController player = PlayerController.instance;
			Debug.Log(DoorManager.instance.getDoor(toDoor));
			Vector3 movePos = DoorManager.instance.getDoor(toDoor).gameObject.transform.position - Vector3.forward * 2;
			player.transform.position = movePos;
			move = false;
		}
	}

	public void MovePlayerToDoor(PlayerController player, string door) {
		toDoor = door;
		move = true;
	}
}

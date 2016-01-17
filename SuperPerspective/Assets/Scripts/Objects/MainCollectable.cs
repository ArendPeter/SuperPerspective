using UnityEngine;
using System.Collections;

public class MainCollectable : Interactable {

	#pragma warning disable 472, 1692

	static int collectableHeld = 0;
	bool active = true;

	void FixedUpdate() {
		if (active)
			GetComponentInChildren<Renderer>().transform.Rotate(Vector3.up, Mathf.PI / 4, Space.World);
	}

	public ParticleSystem effectOnCollect;

	public static bool UseKey(int amtRequired) {
		amtRequired = 1;
		if (collectableHeld >= amtRequired) {
			collectableHeld--;
			return true;
		}
		return false;
	}

	public static void Collect() {
		collectableHeld++;
	}

	public static void GiveMainCollectable(int amt) {
		collectableHeld = collectableHeld + amt;
	}

	public static void ClearMainCollectable() {
		collectableHeld = 0;
	}

	public static int GetMainCollectableHeld(){
		return collectableHeld;
	}

	public override void EnterCollisionWithPlayer () {
		if (!active)
			return;
		base.EnterCollisionWithPlayer();
		Collect();
		//Destroy(GetComponentInChildren<Renderer>());
		if(effectOnCollect != null){
			Instantiate(effectOnCollect, this.transform.position, Quaternion.identity);// PlayerController.instance.transform.position
		}
		gameObject.SetActive(false);
		active = false;
	}
}

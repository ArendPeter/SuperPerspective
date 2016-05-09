//Work In Progress

using UnityEngine;
using System.Collections;

public class CameraRegion : MonoBehaviour {
	public PassiveDynamicCamera passiveCamera;

	void OnTriggerEnter(Collider other){
		if(other.gameObject.GetComponent<PlayerController>() == null)
			return;
		passiveCamera.setActivated(true);
	}

	void OnTriggerExit(Collider other){
		if(other.gameObject.GetComponent<PlayerController>() == null)
			return;
		passiveCamera.setActivated(false);
	}
}

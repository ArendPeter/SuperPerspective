using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	public string sceneName;

	void FixedUpdate() {
		if (Input.anyKey) {
			TransitionManager.instance.SceneTransition(null, "", "Hub");
		}
	}
}

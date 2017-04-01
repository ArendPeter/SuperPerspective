using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	public string sceneName;

	void FixedUpdate() {
		if (Input.anyKeyDown) {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                //Do nothin'
            }
            else
            {
                TransitionManager.instance.SceneTransition(null, "", "Hub");
            }
		}
	}
}

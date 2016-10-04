using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	void FixedUpdate(){
		if(Input.anyKey){
			Application.LoadLevel("TutorialScene_DEMO");
		}
	}
}

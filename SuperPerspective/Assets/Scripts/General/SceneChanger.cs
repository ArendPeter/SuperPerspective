using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	public string sceneName;

	void FixedUpdate(){
		if(Input.anyKey){
			Application.LoadLevel(sceneName);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]

public class CrystalCountText : MonoBehaviour {

	private TextMesh txtmesh;
	public string sceneName;
	public int maxCount;

	// Use this for initialization
	void Start () {
		txtmesh = GetComponent<TextMesh>();
		bool hasFinalCrystal = PlayerPrefs.GetInt(sceneName) == 1;
		int currentCount = 0;

		if(PlayerPrefs.HasKey(this.sceneName+"CollectableList")) {
			string[] colList = PlayerPrefs.GetString(this.sceneName+"CollectableList").Split(","[0]);
			currentCount = colList.Length;
		}

		if((hasFinalCrystal || sceneName == "TutorialScene") && currentCount == this.maxCount){
			txtmesh.text = "All Crystals Found!";
		} else {
			txtmesh.text = "Crystals " + currentCount + "/" + this.maxCount;

			if(hasFinalCrystal) {
				txtmesh.text += " + Final";
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

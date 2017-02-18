using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateIvanEvent : MonoBehaviour
{ 
    public FairyFollow ff;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (PlayerPrefs.GetString("IntroCutsceneFinished") == "true")
        {
            ff.shouldFollow = true;
        }
    }

}

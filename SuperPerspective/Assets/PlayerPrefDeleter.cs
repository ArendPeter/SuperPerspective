using UnityEngine;
using System.Collections;

public class PlayerPrefDeleter : MonoBehaviour {

    public bool deletePrefs = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (deletePrefs)
        {
            PlayerPrefs.DeleteAll();
        }
	}
}

using UnityEngine;
using System.Collections;

public class playerControllerProxy : MonoBehaviour {

    public PlayerController pcont;

	// Use this for initialization
	void Start () {
        pcont = GameObject.FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setCutsceneMode(int set)
    {
        if (set == 0)
        {
            pcont.setCutsceneMode(false);
        }
        else
        {
            pcont.setCutsceneMode(true);
        }
    }

}

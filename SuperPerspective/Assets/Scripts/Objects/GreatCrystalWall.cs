using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatCrystalWall : MonoBehaviour {

    public Activatable[] switches;
    bool grass = false, ice = false, desert = false, activated = false;
    public GameObject c1, c2, c3;

    // Use this for initialization
    void Start() {
        if (PlayerPrefs.GetInt("GrassScene") == 1){
            grass = true;
            c1.SetActive(true);
        }
        if (PlayerPrefs.GetInt("DesertScene") == 1)
        {
            desert = true;
            c2.SetActive(true);
        }
        if (PlayerPrefs.GetInt("IceScene") == 1)
        {
            ice = true;
            c3.SetActive(true);
        }
    }

    public void Activate()
    {
        if (!activated)
        {
            if (grass && ice && desert)
            {
                //Crystal Fairy closing dialogue
                //TODO Larry, call this class's OpenGate function after the dialogue
                OpenGate();
            }
            else
            {
                //Crystal Fairy tells you to gather big crystals
            }
        }
    }

    public void OpenGate()
    {
        for (int i = 0; i < switches.Length; i++)
        {
            switches[i].activated = true;
            activated = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

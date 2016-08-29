﻿using UnityEngine;
using System.Collections;

public class AdviceGiver : MonoBehaviour {

    public string currentLoc;//The text name of the island on which we currently reside.

    //The convoArray is used to hold all of the convoNodes that will be added into the 
    public convoNode[] convoArray = new convoNode[50];
    private Hashtable htab = new Hashtable();

    //This will get the scene TextBox by itself.
    private textBoxScript textbox;

    //This is our player. We find it programmatically.
    PlayerController pcont;

    //This is our player spawner. We find it programmatically.
    [SerializeField]
     PlayerSpawnController pspawn;

	// Use this for initialization
	void Start () {
        textbox = GameObject.FindObjectOfType<textBoxScript>();
        pcont = this.GetComponent<PlayerController>();
        pspawn = this.GetComponent<PlayerSpawnController>();
        currentLoc = pspawn.startDoorName;//We gotta figure out where we're starting, after all.
        populateHashtable();
	}
	
	// Update is called once per frame
	void Update () {
        if(textbox == null)//This is a fix in case we don't find the TextBox on the first frame.
        {
            GameObject.FindObjectOfType<textBoxScript>();
        }
        if (pcont.isDisabled() == false && pcont.isGrounded() == true)//Check to see if we can push the button. Also, we can't push the button in the air.
        {
            if (Input.GetKey(KeyCode.T))
            {
                giveAdvice(currentLoc);
            }
        }

	}

    public void populateHashtable()
    {
        foreach(convoNode i in convoArray)
        {
            htab.Add(i.name, i);
            //print(i.name+" added.");
        }
    }

    public void giveAdvice(string ID)
    {
        
        bool swap = false;
        char[] chArr = ID.ToCharArray();
        for (int i = 0; i<chArr.Length; i++)
        {
            if (chArr[i] == 'e')
            {
                if (chArr[i + 1] == 'n')
                {
                    if (chArr[i + 2] == 'd')
                    {
                        swap = true;
                    }
                }
            }
        }
        if (swap) { ID = ID.Replace("end", "start");  }
        print("result: " + ID);
        
        textBoxScript.instance.startConvo((convoNode)(htab[ID]));
    }
}

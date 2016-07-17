using UnityEngine;
using System.Collections;

public class AdviceGiver : MonoBehaviour {

    public string currentLoc;//The text name of the island on which we currently reside.

    //nameArray & convoArray are linked in size. Make sure they are the same. 
    public string[] nameArray = new string[50];//We find the Index of the name in NameArray...
    public convoNode[] convoArray = new convoNode[50];//And use it to load the ConvoNode in ConvoArray

    //This will get the scene TextBox by itself.
    [SerializeField] private textBoxScript textbox;

    //This is our player. We find it programmatically.
    PlayerController pcont;

    //This is our player spawner. We find it programmatically.
    PlayerSpawnController pspawn;

	// Use this for initialization
	void Start () {
        textbox = GameObject.FindObjectOfType<textBoxScript>();
        pcont = this.GetComponent<PlayerController>();
        pspawn = this.GetComponent<PlayerSpawnController>();
        currentLoc = pspawn.startDoorName;//We gotta figure out where we're starting, after all.
	}
	
	// Update is called once per frame
	void Update () {
        if(textbox == null)//This is a fix in case we don't find the TextBox on the first frame.
        {
            GameObject.FindObjectOfType<textBoxScript>();
        }
        if (pcont.isDisabled() == false && pcont.isGrounded() == true)//Check to see if we can push the button. Also, we can't push the button in the air.
        {
            if (Input.GetKey(KeyCode.R))
            {
                int temp = System.Array.IndexOf(nameArray, currentLoc);
                print(temp);
                giveAdvice(temp);
            }
        }

	}

    public void giveAdvice(int ID)
    {
        textBoxScript.instance.startConvo(convoArray[ID]);
    }
}

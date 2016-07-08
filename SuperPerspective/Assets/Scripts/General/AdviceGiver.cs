using UnityEngine;
using System.Collections;

public class AdviceGiver : MonoBehaviour {

    public string currentLoc;

    public string[] nameArray = new string[50];
    public convoNode[] convoArray = new convoNode[50];

    [SerializeField] private textBoxScript textbox;

    PlayerController pcont;
    PlayerSpawnController pspawn;

	// Use this for initialization
	void Start () {
        textbox = GameObject.FindObjectOfType<textBoxScript>();
        pcont = this.GetComponent<PlayerController>();
        pspawn = this.GetComponent<PlayerSpawnController>();
        currentLoc = pspawn.startDoorName;
	}
	
	// Update is called once per frame
	void Update () {
        if(textbox == null)
        {
            GameObject.FindObjectOfType<textBoxScript>();
        }
        if (pcont.isDisabled() == false)
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

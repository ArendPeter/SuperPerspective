using UnityEngine;
using System.Collections;

public class AdviceGiver : MonoBehaviour {

    public string currentLoc;

    public string[] nameArray = new string[50];
    public convoNode[] convoArray = new convoNode[50];

    private textBoxScript textbox;

    PlayerController pcont;

	// Use this for initialization
	void Start () {
        textbox = GameObject.FindObjectOfType<textBoxScript>();
        pcont = this.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {

        if (pcont.isDisabled() == false)
        {
            if (Input.GetKey(KeyCode.R))
            {
                giveAdvice(System.Array.IndexOf(nameArray, currentLoc));
            }
        }

	}

    public void giveAdvice(int ID)
    {
        textbox.startConvo(convoArray[ID]);
    }
}

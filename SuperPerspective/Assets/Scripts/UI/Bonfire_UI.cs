using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bonfire_UI : MonoBehaviour {

    public GameObject all, selection;
    public Image previewPic;

    public GameObject[] choices;
    public Text[] text;
    public Sprite[] pictures;
    public Sprite noPic;

    public int maxIsle;

    public Color standardCol, selectedCol;

    //KeyCodes for the buttons that work the UI. Can be reset, otherwise I use default values
    public KeyCode up, up2, down, down2, select, select2;

    //Determines how long the choice list is. Will resize the UI ingame. Maximum value of 9 for now, may change later.
    public int maxChoice;

    //TODO
    //Hey Peter, use this to tell which island to send the player to
    public int choice;

    //TODO
    //Hey Peter, use teleportFlag to tell when to send the player to an island
    //Hey Peter, use closeFlag to tell when to back out of the UI. Set it back to false when you're done.
    public bool teleportFlag, closeFlag;

	private bool readyForInput = false, active = false;

    //Used to make my life easier, hooray
    private float x, y, z;
    private Vector3 vec;

	// Use this for initialization
	void Start () {
        up = KeyCode.UpArrow;
        up2 = KeyCode.W;
        down = KeyCode.DownArrow;
        down2 = KeyCode.S;
        select = KeyCode.E;
        select2 = KeyCode.Space;
        reset();
        teleportFlag = false;
        Debug.Log(maxChoice);
        disableExtraChoices();
        ToggleOff();
        maxIsle = 0;
        closeFlag = false;
    }
	
	// Update is called once per frame
	void Update () {
        checkInput();
    }

    //TODO
    //Hey Peter, call this to trigger the UI on
    public void ToggleOn()
    {
        all.SetActive(true);
		active = true;
    }

    //TODO
    //Hey Peter, call this to trigger the UI off
    public void ToggleOff()
    {
        all.SetActive(false);
		active = false;
		readyForInput = false;
    }

    private void checkInput()
    {
		if (!teleportFlag && readyForInput) {
            if (Input.GetKeyDown(select) || Input.GetKeyDown(select2))
            {
                teleportFlag = true;
            }

            else if (Input.GetKeyDown(up) || Input.GetKeyDown(up2))
            {
                moveUp();
                moveSelect();
            }

            else if (Input.GetKeyDown(down) || Input.GetKeyDown(down2))
            {
                moveDown();
                moveSelect();
            }
        }
		if (active && !readyForInput) {
			readyForInput = true;
		}
    }

    private void moveUp ()
    {
        if (choice > 0)
            choice--;
        else
            choice = maxChoice - 1;
        //Debug.Log("Going up! Choice: " + choice);
    }

    private void moveDown()
    {
        if (choice < maxChoice -1)
            choice++;
        else
            choice = 0;
        //Debug.Log("Going down! Choice: " + choice);
    }

    private void moveSelect()
    {
        //Moves the transparent selection UI
        x = selection.transform.position.x;
        y = choices[choice].transform.position.y;
        z = selection.transform.position.z;
        vec = new Vector3(x, y, z);
        selection.transform.position = vec;

        //Sets the text colors
        for (int i = 0; i < maxChoice; i++)
        {
            if (i == choice)
            {
                text[i].color = selectedCol;
                if(pictures.Length > i && pictures[i] != null)
                {
                    previewPic.sprite = pictures[i];
                }
                else
                {
                    previewPic.sprite = noPic;
                }
            }
            else
            {
                text[i].color = standardCol;
            }
        }
    }

    //Disables buttons that excede the choice count
    private void disableExtraChoices()
    {
        for (int i = 0; i < 9; i++)
        {
            if (i >= maxChoice)
            {
                text[i].gameObject.SetActive(false);
                choices[i].SetActive(false);
            }
            else
            {
                text[i].gameObject.SetActive(true);
                choices[i].SetActive(true);
            }
        }
    }

    //TODO
    //Hey Peter, call this after the player exits the menu or warps.
    public void reset()
    {
        choice = 0;
        moveSelect();
    }

    public void exit()
    {
        closeFlag = true;
        reset();
        ToggleOff();
    }

    //TODO Peter update this whenever new island is reached
    public void updateMaxIsle(int newMax)
    {
        maxIsle = newMax;
    }
}

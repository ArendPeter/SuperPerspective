using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bonfire_UI : MonoBehaviour {

    public GameObject all, selection, locked, back;
    public Image previewPic;

    public GameObject[] choices;
    public Text[] text;
    public Text backText;
    public Sprite[] pictures;
    public Sprite noPic;

    //Variable to be updated by Peter
    public int maxIsle = 0;

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

    private bool readyForInput = false, active = false, goUp = false, goDown = false, goSel = false;

    //Used to make my life easier, hooray
    private float x, y, z;
    private Vector3 vec;

    void RegisterEventHandlers()
    {
        InputManager.instance.MenuDownEvent += XboxMenuDown;
        InputManager.instance.MenuUpEvent += XboxMenuUp;
        InputManager.instance.JumpPressedEvent += XboxSelect;
    }

    void XboxMenuDown()
    {
        goDown = true;
    }

    void XboxMenuUp()
    {
        goUp = true;
    }

    void XboxSelect()
    {
        goSel = true;
    }

    // Use this for initialization
    void Start () {
        RegisterEventHandlers();
        up = KeyCode.UpArrow;
        up2 = KeyCode.W;
        down = KeyCode.DownArrow;
        down2 = KeyCode.S;
        //select = KeyCode.E;
        select2 = KeyCode.Space;
        reset();
        teleportFlag = false;
        //Debug.Log(maxChoice);
        disableExtraChoices();
        //ToggleOn();
        ToggleOff();
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
        moveSelect();
        //Debug.Log("ON");
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
		if (closeFlag)
		{
			ToggleOff();
			closeFlag = false;
			return;
		}
		if (!teleportFlag && readyForInput) {
            if (Input.GetKeyDown(select) || Input.GetKeyDown(select2) || goSel)
            {
                Teleport();
            }

            else if (Input.GetKeyDown(up) || Input.GetKeyDown(up2) || goUp)
            {
                moveUp();
                moveSelect();
            }

            else if (Input.GetKeyDown(down) || Input.GetKeyDown(down2) || goDown)
            {
                moveDown();
                moveSelect();
            }
        }
		if (active && !readyForInput) {
			readyForInput = true;
		}
        goUp = false;
        goDown = false;
        goSel = false;
    }

    private void Teleport()
    {
        Debug.Log("Choice: " + choice);
        if (choice == 10)
        {
            Debug.Log("Whoops");
            closeFlag = true;
            active = false;
            readyForInput = false;
        }
        else if (choice + 1 > maxIsle)
        {
            GetComponent<AudioSource>().Play();
        }
        else
        {
            teleportFlag = true;
            active = false;
            readyForInput = false;
        }
    }

    private void moveUp ()
    {
        if (choice == 10)
            choice = maxChoice - 1;
        else if (choice > 0)
            choice--;
        else
            choice = 10;
            //choice = maxChoice - 1;
        //Debug.Log("Going up! Choice: " + choice);
    }

    public void clickChoice(int choice)
    {
        this.choice = choice;
        moveSelect();
        if (active && !readyForInput)
        {
            readyForInput = true;
        }
        goUp = false;
        goDown = false;
        goSel = true;
        checkInput();
    }

    private void moveDown()
    {
        if (choice < maxChoice -1)
            choice++;
        else if (choice == 10)
            choice = 0;
        else
            choice = 10;
        //Debug.Log("Going down! Choice: " + choice);
    }

    private void moveSelect()
    {
        //Moves the transparent selection UI

        if (choice == 10)
        {
            selection.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 32f);
            backText.color = selectedCol;
            vec = back.transform.position;
        }
        else
        {
            selection.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 50f);
            backText.color = standardCol;
            vec = choices[choice].transform.position;
        }

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
                else if (choice != 10)
                {
                    previewPic.sprite = noPic;
                }
            }
            else
            {
                text[i].color = standardCol;
            }
        }

        if (choice != 10)
        {
            if (choice + 1 > maxIsle)
                locked.GetComponent<Image>().enabled = true;
            else
                locked.GetComponent<Image>().enabled = false;
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
        closeFlag = false;
        reset();
        ToggleOff();
    }

    //TODO Peter update this whenever new island is reached
    public void updateMaxIsle(int newMax)
    {
        maxIsle = newMax;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pause_UI : MonoBehaviour {

    public GameObject[] choices;
    public Text[] text;
    public GameObject selection, all;

    public PauseMenu pauseMenu;
    public GameStateManager gs;

    //Adjusts menu if player is in hub
    public bool isHub = false;

    int choice = 0;
    int maxChoice;

    public bool isPaused = false;
    bool active = false;

    public Color standardCol, selectedCol;

    //KeyCodes for the buttons that work the UI. Can be reset, otherwise I use default values
    public KeyCode up, up2, down, down2, select, select2;

    // Use this for initialization
    void Start () {
        up = KeyCode.UpArrow;
        up2 = KeyCode.W;
        down = KeyCode.DownArrow;
        down2 = KeyCode.S;
        select = KeyCode.E;
        select2 = KeyCode.Space;
        maxChoice = choices.Length - 1;
    }
	
	// Update is called once per frame
	void Update () {
        enableDisable();
        checkInput();
    }

    void enableDisable()
    {
        if (isPaused == false && active == true)
        {
            active = false;
            ToggleOff();
        }
        else if (isPaused == true && active == false)
        {
            active = true;
            ToggleOn();
        }
    }

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
    }


    private void checkInput()
    {
        if (active)
        {
            if (Input.GetKeyDown(select) || Input.GetKeyDown(select2))
            {
                if (choice == 2 || (choice == 1 && isHub))
                {
                    quitGame();
                }
                else if (choice == 1)
                {
                    returnToHUB();
                }
                else
                {
                    gs.HandlePausePressed();
                }

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
    }

    private void moveUp()
    {
        if (choice > 0)
            choice--;
        else
            choice = maxChoice;
        //choice = maxChoice - 1;
        //Debug.Log("Going up! Choice: " + choice);
    }

    private void moveDown()
    {
        if (choice < maxChoice)
            choice++;
        else
            choice = 0;
        //Debug.Log("Going down! Choice: " + choice);
    }

    private void moveSelect()
    {
        //Moves the transparent selection UI


        Vector3 vec = choices[choice].transform.position;
        selection.transform.position = vec;

        //Sets the text colors
        for (int i = 0; i <= maxChoice; i++)
        {
            if (i == choice)
            {
                text[i].color = selectedCol;
            }
            else
            {
                text[i].color = standardCol;
            }
        }
    }

    public void returnToHUB()
    {
        pauseMenu.returnToHUB();
    }

    public void quitGame()
    {
        pauseMenu.quitGame();
    }
}

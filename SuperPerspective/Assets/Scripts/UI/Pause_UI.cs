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

    public UISFXManager uiSFX;

    //KeyCodes for the buttons that work the UI. Can be reset, otherwise I use default values
    public KeyCode up, up2, down, down2, select, select2, select3;

    private bool goUp = false, goDown = false, goSel = false;

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
        select = KeyCode.E;
        select2 = KeyCode.Space;
        select3 = KeyCode.Return;
        maxChoice = choices.Length - 1;
        uiSFX = FindObjectOfType<UISFXManager>();
        gs = GameStateManager.instance;
        pauseMenu = GameStateManager.instance.GetComponent<PauseMenu>();
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
            if (goSel)
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

            else if (goUp)
            {
                if (uiSFX != null)
                {
                    uiSFX.PlayMenuMove();
                }
                moveUp();
                moveSelect();
            }

            else if (goDown)
            {
                if (uiSFX != null)
                {
                    uiSFX.PlayMenuMove();
                }
                moveDown();
                moveSelect();
            }
        }
        goUp = false;
        goDown = false;
        goSel = false;
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

    public void pausePressed()
    {
        gs.HandlePausePressed();
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

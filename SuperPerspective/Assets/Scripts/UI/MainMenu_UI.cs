using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_UI : MonoBehaviour {

    public GameObject[] choicesMain, choicesEraseSave, choicesDataCleared;
    public Text[] textMain, textEraseSave, textDataCleared;

    GameObject[] choices;
    Text[] text;
    GameObject selection;

    public GameObject selectionMain, selectionEraseSave, selectionDataCleared;

    public Color standardCol, selectedCol;

    public GameObject initText, DefaultMenu, EraseSaveMenu, DataClearedMenu, panel;
    enum MenuState { init, defaultMenu, eraseSaveMenu, dataClearedMenu, transition };
    MenuState menuState;

    //bools for xbox controller
    bool goDown, goUp, goSel;

    //KeyCodes for the buttons that work the UI. Can be reset, otherwise I use default values
    KeyCode up, up2, down, down2, select, select2, select3;

    int choice = 0;
    int maxChoice = 2;

    bool waitForRelease = false;

    public UISFXManager uiSFX;

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
    void Start() {
        menuState = MenuState.init;
        selection = selectionMain;
        RegisterEventHandlers();
        up = KeyCode.UpArrow;
        up2 = KeyCode.W;
        down = KeyCode.DownArrow;
        down2 = KeyCode.S;
        select = KeyCode.E;
        select2 = KeyCode.Space;
        select3 = KeyCode.Return;
    }

    // Update is called once per frame
    void Update() {
        switch (menuState)
        {
            case MenuState.init:
                InitUpdate();
                break;
            case MenuState.transition:
                break;
            default:
                MenuUpdate();
                break;
        }
    }

    void InitUpdate()
    {
        if (menuState == MenuState.init)
        {
            if (Input.anyKeyDown)
            {
                AnyKeyPressed();
            }
        }
    }

    void MenuUpdate()
    {
        if (!waitForRelease && goSel)
        {
            switch (menuState)
            {
                case MenuState.defaultMenu:
                    if (choice == 2)
                    {
                        QuitGame();
                    }
                    else if (choice == 1)
                    {
                        DeleteSave();
                    }
                    else
                    {
                        StartGame();
                    }
                    break;
                case MenuState.eraseSaveMenu:
                    if (choice == 1)
                    {
                        ClearSaveNo();
                    }
                    else
                    {
                        ClearSaveYes();
                    }
                    break;
                case MenuState.dataClearedMenu:
                    ClearSaveOkay();
                    break;
            }
        }

        else if (goUp)
        {
            if (menuState != MenuState.dataClearedMenu)
            {
                if (uiSFX != null)
                {
                    uiSFX.PlayMenuMove();
                }
                moveUp();
                moveSelect();
            }
        }

        else if (goDown)
        {
            if (menuState != MenuState.dataClearedMenu)
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
        waitForRelease = false;
    }

    private void moveUp()
    {
        if (choice > 0)
            choice--;
        else
            choice = maxChoice - 1;
        //choice = maxChoice - 1;
        //Debug.Log("Going up! Choice: " + choice);
    }

    private void moveDown()
    {
        if (choice < maxChoice - 1)
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
        for (int i = 0; i < maxChoice; i++)
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

    public void AnyKeyPressed()
    {
        menuState = MenuState.defaultMenu;
        selection = selectionMain;
        choice = 0;
        choices = choicesMain;
        text = textMain;
        maxChoice = choices.Length;
        moveSelect();
        initText.SetActive(false);
        DefaultMenu.SetActive(true);
        panel.SetActive(true);
        waitForRelease = true;
    }

    //Default choices
    public void StartGame()
    {
        menuState = MenuState.transition;
        DefaultMenu.SetActive(false);
        panel.SetActive(false);
        TransitionManager.instance.SceneTransition(null, "", "Hub");
    }

    public void DeleteSave()
    {
        SteamManager.Instance.ResetGame();
        menuState = MenuState.eraseSaveMenu;
        selection = selectionEraseSave;
        choice = 0;
        choices = choicesEraseSave;
        text = textEraseSave;
        maxChoice = choices.Length;
        moveSelect();
        DefaultMenu.SetActive(false);
        EraseSaveMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //Save choices
    public void ClearSaveYes()
    {
        PlayerPrefs.DeleteAll();
        menuState = MenuState.dataClearedMenu;
        choice = 0;
        selection = selectionDataCleared;
        choices = choicesDataCleared;
        text = textDataCleared;
        maxChoice = choices.Length;
        moveSelect();
        EraseSaveMenu.SetActive(false);
        DataClearedMenu.SetActive(true);
    }

    public void ClearSaveNo()
    {
        menuState = MenuState.defaultMenu;
        selection = selectionMain;
        choice = 0;
        choices = choicesMain;
        text = textMain;
        maxChoice = choices.Length;
        moveSelect();
        EraseSaveMenu.SetActive(false);
        DefaultMenu.SetActive(true);
    }

    //Post save clear choice
    public void ClearSaveOkay()
    {
        menuState = MenuState.defaultMenu;
        selection = selectionMain;
        choice = 0;
        choices = choicesMain;
        text = textMain;
        maxChoice = choices.Length;
        moveSelect();
        DataClearedMenu.SetActive(false);
        DefaultMenu.SetActive(true);
    }
}

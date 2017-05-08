using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_UI : MonoBehaviour {

    public GameObject initText, DefaultMenu, EraseSaveMenu, DataClearedMenu, panel;
    enum MenuState { init, defaultMenu, eraseSaveMenu, dataClearedMenu, transition };
    MenuState menuState;


    // Use this for initialization
    void Start() {
        menuState = MenuState.init;
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

    }

    public void AnyKeyPressed()
    {
        menuState = MenuState.defaultMenu;
        initText.SetActive(false);
        DefaultMenu.SetActive(true);
        panel.SetActive(true);
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
        menuState = MenuState.eraseSaveMenu;
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
        EraseSaveMenu.SetActive(false);
        DataClearedMenu.SetActive(true);
    }

    public void ClearSaveNo()
    {
        menuState = MenuState.defaultMenu;
        EraseSaveMenu.SetActive(false);
        DefaultMenu.SetActive(true);
    }

    //Post save clear choice
    public void ClearSaveOkay()
    {
        menuState = MenuState.defaultMenu;
        DataClearedMenu.SetActive(false);
        DefaultMenu.SetActive(true);
    }
}

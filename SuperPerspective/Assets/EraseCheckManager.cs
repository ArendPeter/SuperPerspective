using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EraseCheckManager : MonoBehaviour {

    public bool showPanel = false;
    public GameObject CheckPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

            CheckPanel.SetActive(showPanel);
	}

    public void setPanelVisibility(bool set)
    {
        showPanel = set;
    }

    public void eraseData()
    {
        PlayerPrefs.DeleteAll();
        setPanelVisibility(false);
    }

    public void cancelErase()
    {
        setPanelVisibility(false);
    }
}

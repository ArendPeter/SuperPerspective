using UnityEngine;
using System.Collections;

public class DevConsoleController : MonoBehaviour {

	public static DevConsoleController instance;
	GameObject devConsole;

	// Use this for initialization
	void Awake() {
		if(instance == null)
			instance = this;
	}

	void Start () {
		devConsole = GameObject.Find("Console_Menu");

		if(devConsole != null) {
			InputManager.instance.DevConsoleEvent += ToggleDevConsole;
			devConsole.transform.position = Vector3.zero;
			devConsole.SetActive(false);
		} else {
			//print("WARNING: Console_Menu not found!");
		}
	}

	public void ToggleDevConsole(){
        //Disabled for release -Nick
        /*
		if(isConsoleActive()){
			devConsole.SetActive(false);
		}else{
			devConsole.SetActive(true);
		}*/
	}

	public bool isConsoleActive(){
		return devConsole.activeInHierarchy;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

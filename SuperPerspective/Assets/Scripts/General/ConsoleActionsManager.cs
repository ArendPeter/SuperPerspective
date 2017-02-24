using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConsoleActionsManager : MonoBehaviour {
	GameObject canvas;
	GameObject player;
	PlayerSpawnController psc;
	InputField dcl;

	// Use this for initialization
	void Start () {
		this.init();
	}

	void init(){
		//canvas = GameObject.Find("Console Menu") as GameObject;
		player = GameObject.FindWithTag("Player");
		psc = player.GetComponent<PlayerSpawnController>();
		dcl = GameObject.Find("DevCommandLine").GetComponent<InputField>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("return") && dcl.text != ""){
			this.consoleCommand(dcl.text);
		}
	}

	//will do something based on the command (param c)
	public void consoleCommand(string c){
		// Debug.Log("executing command " + c + "...");
		string[] commandArray = c.Split(" "[0]);

		string debugArray = "";
		foreach(string s in commandArray){
			debugArray = debugArray + s + ", ";
		}

		string command = commandArray[0];
		string val = "0";
		if(commandArray.Length > 1){
			val = commandArray[1];
		}

		if(command != null){
			switch(command){
				case "givekey":
					Key.GiveKeys(int.Parse(val));
					//Debug.Log("Console: giving player " + val + " keys");
					break;
				case "tp":
					movePlayer(val);
					//Debug.Log("Console: moving player to door " + val);
					break;
				case "hub":
					moveToScene(val);
					//Debug.Log("Console: moving player to scene " + val);
					break;
				case "credits":
					dcl.text = "Nick Shooter, Peter Aquila, Larry Smith, Arend Peter Castelein, Daniel Xiao - Thank you!";
					break;
				case "resetprefs":
					PlayerPrefs.DeleteAll();
					//Debug.Log("Console: resetting player prefs.");
					break;
				default:
					//Debug.Log("Console: command " + command + " not found ");
					break;
			}
		}
	}

	//moves player to specific door
	public void movePlayer(string doorName){
		psc.moveToDoor(
			psc.findDoor(doorName)
		);
	}

	public void moveToScene(string sceneName) {
		if(sceneName == null) {
			sceneName = "Hub";
		};

		psc.moveToScene(sceneName);
	}

	//sets player to their default door position
	public void resetPlayer(){
		psc.moveToDoor(psc.getDefaultDest());
	}
}

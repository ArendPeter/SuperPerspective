﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class CheckpointManager : MonoBehaviour {
	//suppress warnings
	#pragma warning disable 414
	
	static int destination = -1;

	public Canvas menu;
	public GameObject marker;
	Image map;

	public static CheckpointManager instance;
	public bool menuVisible = false;

	GameObject[] buttons;

	static Vector2[] points;//positions on map
	static string[] scenes; //names of scenes which markers link to
	static bool[] pointReached;

	int recentPoint = -1;

	float menuAlpha = 0;
	float fadeTime = 1f;

	void Awake(){
		//sigleton pattern
		if(instance == null)
			instance = this;
		else if(instance != this){
			Destroy(gameObject);
			return;
		}
		
		//read in data
		if(scenes == null){
			StreamReader reader = new StreamReader("checkpoints.txt");
			string line = reader.ReadLine();
			int numPoints = System.Int32.Parse(line);

			scenes = new string[numPoints];
			points = new Vector2[numPoints];
			pointReached = new bool[numPoints];

			string[] splitter = {" "};
			for(int i = 0; i<numPoints; i++){
				line = reader.ReadLine();
				string[] splits = line.Split(splitter,3,System.StringSplitOptions.None);
				scenes[i] = splits[0];
				points[i] = new Vector2(
					System.Int32.Parse(splits[1]),
					System.Int32.Parse(splits[2])
				);
			}
			reader.Close();
		}

		//move to destination if we fast travelled
		//movePlayerTo(destination);
	}

	// Use this for initialization
	void Start () {
		//register interact button to leaving the menu
		InputManager.instance.InteractPressedEvent += exitMenu;
		//init menu
		initMenu();
	}
	
	// Update is called once per frame
	void Update () {
		//enable/disable canvas component
		menu.GetComponent<Canvas>().enabled = (menuAlpha != 0f);
		//check if we entered new room
		if(destination != -1 && menuVisible){
			menuAlpha = 1;
			destination = -1;
		}
		//update alpha
		menuAlpha += ((menuVisible)? (1/fadeTime) : -(1/fadeTime))*Time.deltaTime;
		menuAlpha = Mathf.Clamp(menuAlpha,0f,1f);
		menu.GetComponent<CanvasGroup>().alpha = menuAlpha;
	}

	public void showMenu(int id){
		pointReached[id] = true;
		SaveManager.instance.addPointReached(id);
		SaveManager.instance.setRecentPoint(id);
		showMenu();
	}

	public void showMenu(){
		menuVisible = true;
		for(int i = 0; i < buttons.Length; i++)
			if(pointReached[i])
				buttons[i].SetActive(true);
	}

	public void exitMenu(){
		if(menuAlpha<.5f)
			return;
		menuVisible = false;
		for(int i = 0; i < buttons.Length; i++)
			buttons[i].SetActive(false);
	}

	void initMenu(){
		buttons = new GameObject[points.Length];
		//get map
		map = menu.transform.FindChild("Map").GetComponent<Image>();
		float mapX = map.transform.position.x;
		float mapY = map.transform.position.y;
		//add checkpoints to map
		for(int i= 0; i < points.Length;i++){
			//create button
			buttons[i] = Instantiate(marker, new Vector3(0,0,0), Quaternion.identity) as GameObject;
			//position
			buttons[i].transform.SetParent(map.transform);
			Vector3 pos = buttons[i].transform.position;
			pos.x = mapX + points[i].x;
			pos.y = mapY + points[i].y;
			buttons[i].transform.position = pos;
			//ad listenes
			int j = i;
			buttons[i].GetComponent<Button>().onClick.AddListener(() => { goToCheckPoint(j);});
			buttons[i].SetActive(false);
		}
	}

	void movePlayerTo(int dest){
		if(destination == -1)
			return;
		//search for correct checkpoint
		GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
		GameObject player = GameObject.Find("Player");
		bool checkpointFound = false;
		for(int i = 0; i < cps.Length; i++){
			if(cps[i].GetComponent<Checkpoint>().id == destination){
				//player.transform.position = cps[i].transform.position;
				player.GetComponent<PlayerController>().Teleport(cps[i].transform.position);
				checkpointFound = true;
			}
		}
		if(!checkpointFound){
			//Debug.Log("You Lied >:( (i.e. Destination scene does not contain the correct checkpoint)");
		}
	}
	
	public void goToCheckPoint(int id){
		destination = id;
		exitMenu();
		movePlayerTo(destination);
		//Application.LoadLevel(scenes[id]);
	}

	public void setPointReached(bool[] p){
		pointReached = p;
	}

	public void setRecentPoint(int r){
		recentPoint = r;
	}

	public int getNumCheckpoints(){
		return points.Length;
	}

}

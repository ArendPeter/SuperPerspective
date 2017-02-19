using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (GameObject))]

public class Saved_UI : MonoBehaviour {

	public static Saved_UI instance = null;
	public GameObject SavedUIGraphic;

	private int Timer = 0;
	private int defaultPopupTime = 200;

	private GameObject UIObject;
	private GameObject canvas;

	void Awake() {
		if(instance == null){
			instance = this;
		}
		canvas = GameObject.Find("Bonfire_UI");
	}

	void Update() {
		if(Timer > 0 && UIObject != null){
			Timer--;
			
			int animateTime = 26;
			int animateSpeed = 220;
			//go up
			if(Timer > defaultPopupTime - animateTime){
				UIObject.transform.position = new Vector2(UIObject.transform.position.x, UIObject.transform.position.y + animateSpeed * Time.deltaTime);
			} else if(Timer < 0 + animateTime){
				UIObject.transform.position = new Vector2(UIObject.transform.position.x, UIObject.transform.position.y - animateSpeed * Time.deltaTime);
			}

		}
		if(Timer == 1){
			Destroy(UIObject);
		}
	}

	public void ShowSaved() {
		if(UIObject == null){
        	UIObject = Instantiate(SavedUIGraphic);
		}
		Timer = defaultPopupTime;

        UIObject.transform.parent = canvas.transform;
        UIObject.transform.position = new Vector2(800, -50);
	}
}

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
		this.Timer = 0;
		canvas = GameObject.Find("Bonfire_UI");
	}

	void Update() {
		if(this.Timer > 0 && this.UIObject != null){
			this.Timer--;
			
			int animateTime = 10;
			int animateSpeed = 240;
			//go up
			if(this.Timer > defaultPopupTime - animateTime){
				this.UIObject.transform.position = new Vector2(this.UIObject.transform.position.x, this.UIObject.transform.position.y + animateSpeed * Time.deltaTime);
			} else if(this.Timer < 0 + animateTime){
				this.UIObject.transform.position = new Vector2(this.UIObject.transform.position.x, this.UIObject.transform.position.y - animateSpeed * Time.deltaTime);
			}

		}
		if(this.Timer == 1){
			Destroy(this.UIObject);
		}
	}

	public void ShowSaved() {
		if(this.UIObject == null){
        	this.UIObject = Instantiate(SavedUIGraphic);
		}
		if(this.Timer <= 0){
			this.Timer = defaultPopupTime;

	        this.UIObject.transform.SetParent(canvas.transform, false);
	        this.UIObject.transform.position = new Vector2(400, -50);
		}
	}
}

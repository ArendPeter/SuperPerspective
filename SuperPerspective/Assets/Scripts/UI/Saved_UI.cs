using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (GameObject))]

public class Saved_UI : MonoBehaviour {

	public static Saved_UI instance = null;
	public GameObject SavedUIGraphic;

	private int Timer = 0;
	private int defaultPopupTime = 150;

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

			int timerHalf = this.defaultPopupTime / 2;
			int timerWait = 30;
			
			Vector2 startPos = new Vector2(100, -55);
			Vector3 topPos = new Vector2(100, 125);

			float timerMeasure = Mathf.Abs(this.Timer - timerHalf);
			float timerPercentage = (timerMeasure / timerHalf);

			//go up
			if(this.Timer > (timerHalf + timerWait)){
				this.UIObject.transform.position = Vector2.Lerp(topPos, startPos, timerPercentage);
			} else if(this.Timer < (timerHalf - timerWait) ){
				this.UIObject.transform.position = Vector2.Lerp(topPos, startPos, timerPercentage);
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
		}
	}
}

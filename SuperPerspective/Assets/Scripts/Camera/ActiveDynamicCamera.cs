using UnityEngine;
using System.Collections;

public class ActiveDynamicCamera : Activatable {

	public bool panOnActivate = true;
	public bool panOnDeactivate = false;
	public bool deactivateOnReset = false;

	public Texture letterboxTexture;
	public float letterboxSpeed = 0.2f;

	public float panDuration = 3;

	private float panTime = -1f, letterboxParam;
	private bool panned = false;

	public void FixedUpdate(){
		checkForLockEnd();
	}

	private void checkForLockEnd(){
		bool stillWaiting = panTime > 0;
		bool timesUp = Time.time - panTime > panDuration;
		bool dynamicStateExited = !GameStateManager.targetingDynamicState();
		if(stillWaiting && (timesUp || dynamicStateExited)){
			resetPan();
			if(deactivateOnReset){
				base.setActivated(false);
			}
		}
	}

	void OnGUI() {
		if (panned && letterboxParam < Mathf.PI / 4) {
			letterboxParam += Mathf.PI * letterboxSpeed * Time.deltaTime;
		} else if (!panned && letterboxParam > 0) {
			letterboxParam -= Mathf.PI * letterboxSpeed * Time.deltaTime;
		}
		if (letterboxParam < 0)
			letterboxParam = 0;
		if (letterboxParam > Mathf.PI / 4)
			letterboxParam = Mathf.PI / 4;
		if (letterboxParam > 0) {
			GUI.enabled = true;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height / 5.0f * Mathf.Sin(letterboxParam)), letterboxTexture);
			GUI.DrawTexture(new Rect(0, Screen.height - (Screen.height / 5.0f * Mathf.Sin(letterboxParam)), Screen.width, Screen.height / 5.0f), letterboxTexture);
		}
	}

	public override void setActivated(bool a){
		bool startA = activated;
		base.setActivated(a);
		if(a == startA)
			return;
		if(!panned && ((a && panOnActivate) || (!a && panOnDeactivate))){
			GameStateManager.instance.EnterDynamicState(transform);
			bool dynamicStateEntered = GameStateManager.targetingDynamicState();
			if(dynamicStateEntered){
				PlayerController.instance.setCutsceneMode(true);
				panTime = Time.time;
				panned = true;
			}else if(deactivateOnReset){
				base.setActivated(false);
			}
		}
	}

	private void resetPan(){
		panned = false;
		GameStateManager.instance.ExitDynamicState();
		PlayerController.instance.setCutsceneMode(false);
		panTime = -1f;
	}
}

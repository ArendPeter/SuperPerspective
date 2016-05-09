using UnityEngine;
using System.Collections;

public class ActiveDynamicCamera : Activatable {

	public bool panOnActivate = true;
	public bool panOnDeactivate = false;
	public bool deactivateOnReset = false;

	public float panDuration = 3;

	private float panTime = -1f;
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

using UnityEngine;
using System.Collections;

public class RealmTracker : Activatable {

	public bool shiftTriggerable = false;
	
	public GameObject[] reds,blues;
	
	void Start(){
		GameStateManager.instance.PerspectiveShiftSuccessEvent += perspectiveShift;
	}
	
	public override void setActivated(bool a){
		RealmManager.instance.toggle(RealmManager.getDimension() == 1? false : true);
	}
	
	public void perspectiveShift(){
		if(shiftTriggerable){
			RealmManager.instance.updateBlockGeometry(GameStateManager.instance.currentPerspective);
			setActivated(RealmManager.getDimension() == 1? false : true);
		}
	}
}

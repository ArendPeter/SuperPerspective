using UnityEngine;
using System.Collections;

public class Mover : Activatable {
	public Vector3 movement = Vector3.zero;//path along which object will move
	public float transitionTime = 1f;//time it takes for transition to occur

	Vector3 startPosition;//start position
	float prog = 0f; //progression from start to start+ movement

    GameObject moverSFX;
    SwitchMoverSFX smSFX;

	void Start(){
		//make startPositon to be current position
		Vector3 pos = transform.localPosition;
		startPosition = new Vector3(pos.x,pos.y,pos.z);
        moverSFX = Instantiate(Resources.Load("Sound/SwitchMoverSFX") as GameObject);
        moverSFX.transform.parent = gameObject.transform;
        smSFX = moverSFX.GetComponent<SwitchMoverSFX>();
    }

	void Update(){
		if (!PlayerController.instance.isPaused()){
            //update prog

            prog += (Time.deltaTime/transitionTime) * ((activated)? 1 : -1);//increase or decrease depending on activated
			prog = Mathf.Clamp01(prog); //clamp between 0 and 1

            if (activated)
            {
                if (prog == 1)
                {
                    smSFX.StopSFX();
                }
                else
                {
                    smSFX.StartSFX();
                }
            }
            if (!activated)
            {
                if (prog == 0)
                {
                    smSFX.StopSFX();
                }
                else
                {
                    smSFX.StartSFX();
                }
            }

            //set position
            transform.localPosition = Vector3.Lerp(startPosition, startPosition + movement, prog);
        }
	}
}

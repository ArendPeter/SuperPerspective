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

	bool CrushingPlayerBelow(){
		if(GetComponent<Collider>() == null)
			return false;
		Bounds myBounds = GetComponent<Collider>().bounds;
		Bounds playerBounds = PlayerController.instance.GetComponent<Collider>().bounds;
		Rect playerHorBounds = new Rect(playerBounds.min.x, playerBounds.min.z,
		 		playerBounds.size.x, playerBounds.size.z);
		Rect myHorBounds = new Rect(myBounds.min.x,myBounds.min.z,myBounds.size.x, myBounds.size.z);
		bool horizontalOverlap = myHorBounds.Overlaps(playerHorBounds);
		float spaceAboveHead = myBounds.min.y-playerBounds.max.y;
		if(spaceAboveHead < 0){//this corresponds to the box being below the player
			spaceAboveHead = 1e10f;
		}
		bool movingDown = (activated)? (movement.y < 0) : (movement.y > 0);
		return horizontalOverlap && spaceAboveHead < .5 && movingDown;
	}

	bool PushingPlayer(){
		if(GetComponent<Collider>() == null)
			return false;
		Bounds myBounds = GetComponent<Collider>().bounds;
		Bounds playerBounds = PlayerController.instance.GetComponent<Collider>().bounds;
		bool isPushing = false;
		for(int axis = 0; axis < 3; axis+=2){//loop through x and z axis
			int otherAxis = 2 - axis;
			Rect playerSideBounds = new Rect(playerBounds.min[otherAxis], playerBounds.min.y,
			 		playerBounds.size[otherAxis], playerBounds.size.y);
			Rect mySideBounds = new Rect(myBounds.min[otherAxis],myBounds.min.y,myBounds.size[otherAxis], myBounds.size.y);
			bool overlaps = mySideBounds.Overlaps(playerSideBounds);
			bool movingTowardPlayer = Mathf.Abs(movement[axis]) > 0.01 &&
				(Mathf.Sign(movement[axis]) * (activated?1:-1)) ==
				Mathf.Sign(playerBounds.center[axis] -myBounds.center[axis]);
			float dist = Mathf.Max(
				myBounds.min[axis]-playerBounds.max[axis],
				playerBounds.min[axis]-myBounds.max[axis]);
			if(dist < 0){
				dist = 100;
			}
			isPushing = isPushing || (overlaps && dist < .5 && movingTowardPlayer);
		}
		return isPushing;
	}

	bool MovingPlatformInWay(){
		if(GetComponent<Collider>() == null)
			return false;
		Bounds myBounds = GetComponent<Collider>().bounds;
		bool isPushing = false;
		foreach(MobilePlatform plat in Object.FindObjectsOfType(typeof(MobilePlatform)) as MobilePlatform[]){
			Bounds platBounds = plat.GetComponent<Collider>().bounds;
			for(int axis = 0; axis < 3; axis++){
				int[] otherAxis = new int[2];
				otherAxis[0] = (axis == 0)? 1 : 0;
				otherAxis[1] = (axis == 2)? 1 : 2;
				Rect platSideBounds = new Rect(platBounds.min[otherAxis[0]], platBounds.min[otherAxis[1]],
				 		platBounds.size[otherAxis[0]], platBounds.size[otherAxis[1]]);
				Rect mySideBounds = new Rect(myBounds.min[otherAxis[0]], myBounds.min[otherAxis[1]],
				 		myBounds.size[otherAxis[0]], myBounds.size[otherAxis[1]]);
				bool overlaps = mySideBounds.Overlaps(platSideBounds);
				bool movingTowardPlayer = Mathf.Abs(movement[axis]) > 0.01 &&
					(Mathf.Sign(movement[axis]) * (activated?1:-1)) ==
					Mathf.Sign(platBounds.center[axis] - myBounds.center[axis]);
				float dist = Mathf.Max(
					myBounds.min[axis]-platBounds.max[axis],
					platBounds.min[axis]-myBounds.max[axis]);
				if(dist < 0){
					dist = 100;
				}
				isPushing = isPushing || (overlaps && dist < .5 && movingTowardPlayer);
			}
		}
		return isPushing;
	}

	void Update(){
		bool somethingInWay = CrushingPlayerBelow() || PushingPlayer() || MovingPlatformInWay();
		if (!PlayerController.instance.isPaused() && !somethingInWay){
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

using UnityEngine;
using System.Collections;

public class Orb : ActiveInteractable {

	//suppress warnings
	#pragma warning disable 414

	public float followSpeed = 2.1f;
	public Vector3 posOnPlayer = new Vector3(0,2,0);

	public float resetTime = 2.0f;
	private float dropTime;
	private Vector3 dropPosition;

	private bool isHeld = false;

	private float distThresh = 1.5f; //distance threshhold where it will become unpressed

	private Vector3 startPos;

	void Start() {
		base.StartSetup ();
		range = 1.5f;
		startPos = new Vector3(
			transform.position.x,
			transform.position.y,
			transform.position.z
		);
		transform.parent = null;
	}

	public void FixedUpdate(){
		base.FixedUpdateLogic();

		if(!isHeld && !AtStart()){
			updateRecallPosition();
		}
	}

	public void Update(){
		if(isHeld) {
			FollowPlayer();
		}
	}

	private void FollowPlayer(){
		Vector3 targetPos = PlayerController.instance.transform.position + posOnPlayer;
		float dist = Vector3.Distance(transform.position, targetPos);

		if(dist >= distThresh) {
			transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed*Time.deltaTime);
		}
	}

	private void updateRecallPosition(){
		float travelTime = Time.time - dropTime;
		float percent = travelTime / resetTime;
		transform.position = Vector3.Lerp(
				dropPosition, startPos, percent);
	}

	public override float GetDistance() {
		if(PlayerController.instance.isHoldingOrb() || !AtStart()){
			return float.MaxValue;
		}else{
			if (GameStateManager.is3D())
				return Vector3.Distance(transform.position, player.transform.position);
			else
				return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
				                        new Vector2(transform.position.x, transform.position.y));
		}
	}

	public override void Triggered(){
		PickUp();
	}

	private void PickUp(){
		PlayerController.instance.grabOrb(this);
		isHeld = true;

		Vector3 pos = PlayerController.instance.transform.position;
		pos += posOnPlayer;
		transform.position = pos;

		//transform.parent = PlayerController.instance.transform;
	}

	public void Drop(){
		isHeld = false;
		transform.parent = null;
		dropPosition = new Vector3(
			transform.position.x,
			transform.position.y,
			transform.position.z
		);
		dropTime = Time.time;
	}

	private bool AtStart(){
		return startPos == transform.position;
	}
}

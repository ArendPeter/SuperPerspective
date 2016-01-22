using UnityEngine;
using System.Collections;

public class Orb : ActiveInteractable {
	//suppress warnings
	#pragma warning disable 414

	public ParticleSystem trailParticle;
	public GameObject model;

	public float minFollowSpeed;
	public Vector3 posOnPlayer;

	public float dropOutwardSpeed;
	public float dropGravity;
	private float dropDistBelowPlayer = -1f;
	private float dropVerticalSpeed = 0f;
	private Vector2 dropVector;

	public float resetSpeed;

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
		if(trailParticle != null){
			trailParticle.enableEmission = false;
		}
	}

	private void PickUp(){
		PlayerController.instance.grabOrb(this);
		isHeld = true;
	}

	//Update
	public void FixedUpdate(){
		base.FixedUpdateLogic();
		if(!AtStart() && !OrbBroken()){
			dropVerticalSpeed += dropGravity;
		}
	}

	public void Update(){
		if(isHeld){
			FollowPlayer();
		}else if(!AtStart()){
			if(!OrbBroken()){
				UpdateFallPosition();
			}else{
				UpdateRecallPosition();
			}
		}
	}

	private void FollowPlayer(){
		Vector3 targetPos = PlayerController.instance.transform.position + posOnPlayer;
		float dist = Vector3.Distance(transform.position, targetPos);
		float playerSpeed = PlayerController.instance.getSpeed();
		float speed = Mathf.Max(minFollowSpeed,(dist/distThresh) * playerSpeed);
		float percent = speed * Time.deltaTime / dist;
		transform.position = Vector3.Lerp(transform.position, targetPos, percent);
	}

	private void UpdateFallPosition(){
		Vector3 pos = transform.position;
		pos += new Vector3(
			dropOutwardSpeed * dropVector.x,
			dropVerticalSpeed,
			dropOutwardSpeed * dropVector.y
		) * Time.deltaTime;
		transform.position = pos;

		float breakingPoint =
			PlayerController.instance.transform.position.y +
			dropDistBelowPlayer;
		if(pos.y < breakingPoint ){
			pos.y = breakingPoint;
			SetVisible(false);
		}
	}

	private void UpdateRecallPosition(){
		float dist = Vector3.Distance(transform.position, startPos);
		float percent = resetSpeed * Time.deltaTime / dist;
		transform.position = Vector3.Lerp(
				transform.position, startPos, percent);
		if(trailParticle != null){
			trailParticle.enableEmission = true;
		}

		if(AtStart()){
			SetVisible(true);
			if(trailParticle!=null){
				trailParticle.enableEmission = false;
			}
		}
	}

	//convenience
	private bool OrbBroken(){
		if(model != null){
			return !model.GetComponent<Renderer>().enabled;
		}
		return false;
	}

	private void SetVisible(bool vis){
		if(model != null){
			model.GetComponent<Renderer>().enabled = vis;
		}
	}

	private bool AtStart(){
		return startPos == transform.position;
	}

	//inherited
	public override void Triggered(){
		PickUp();
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

	//public interface
	public void Drop(){
		isHeld = false;
		transform.parent = null;
		dropVerticalSpeed = 0f;
	}

	public void SetOutwardDropVector(Vector2 dropVector){
		this.dropVector = dropVector;
	}
}

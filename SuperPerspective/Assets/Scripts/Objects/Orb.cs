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
	public float placeSpeed;

	private bool isHeld = false;
	private bool initialApproach = false;

	private float distThresh = 1f; //distance threshhold where it will become unpressed

	private Vector3 startPos;

	private OrbDropPedestal destObj = null;

	void Start() {
		base.StartSetup ();
		range = 2f;
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
		destObj = null;
		initialApproach = true;
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
			if(initialApproach){
				SpiralToPlayer();
			}else{
				FollowPlayer();
			}
		}else if(!AtTargetLocation()){
			if(!OrbBroken() && !HasFinalPlatform()){
				UpdateFallPosition();
			}else{
				UpdateRecallPosition();
			}
		}
		UpdateDestinationPedestal();
	}

	private void SpiralToPlayer(){
		Vector3 targetPos = PlayerController.instance.transform.position + posOnPlayer;
		transform.position = SpiralPath.SpiralPositionTo(transform.position, targetPos);

		if(targetPos == transform.position){
			initialApproach = false;
		}
	}

	private void FollowPlayer(){
		Vector3 targetPos = PlayerController.instance.transform.position + posOnPlayer;
		float dist = Vector3.Distance(transform.position, targetPos);
		float speed = Mathf.Max(minFollowSpeed,(dist/distThresh) * minFollowSpeed);
		LerpToPosition(targetPos, speed);
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
		Vector3 targetPos = (HasFinalPlatform())?
			destObj.GetOrbPosition() : startPos;
		LerpToPosition(targetPos,resetSpeed);

		if(!HasFinalPlatform() && trailParticle != null){
			trailParticle.enableEmission = true;
		}

		if(AtTargetLocation()){
			SetVisible(true);
			if(trailParticle!=null){
				trailParticle.enableEmission = false;
			}
		}
	}

	private void UpdateDestinationPedestal(){
		if(PlayerController.instance.isHoldingOrb(this)){
			if(destObj != null){
				destObj.ReleaseOrb();
			}
			destObj = null;
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

	private bool AtTargetLocation(){
		return HasFinalPlatform()? OnFinalPlatform() : AtStart();
	}


	public bool OnFinalPlatform(){
		if(destObj == null){
			return false;
		}else{
			return destObj.GetOrbPosition() == transform.position;
		}
	}

	private bool HasFinalPlatform(){
		return destObj != null;
	}

	private void LerpToPosition(Vector3 newPos, float speed){
		if(!GameStateManager.is3D()){
			Vector3 pos = transform.position;
			pos.z = newPos.z;
			transform.position = pos;
		}
		float dist = Vector3.Distance(transform.position, newPos);
		float percent = speed * Time.deltaTime / dist;
		transform.position = Vector3.Lerp(transform.position, newPos, percent);
	}

	//inherited
	public override void Triggered(){
		PickUp();
	}

	protected override bool IsEnabled(){
		return !PlayerController.instance.isHoldingOrb() && AtTargetLocation();
	}

	public override float GetDistance() {
		if (GameStateManager.is3D())
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
			                        new Vector2(transform.position.x, transform.position.y));
	}

	//public interface
	public void Drop(){
		isHeld = false;
		transform.parent = null;
		dropVerticalSpeed = 0f;
		if(destObj != null){
			destObj.ReleaseOrb();
			destObj = null;
		}
	}

	public void SetOutwardDropVector(Vector2 dropVector){
		this.dropVector = dropVector;
	}

	public void SetPlatform(OrbDropPedestal obj){
		destObj = obj;
	}

	public bool IsOnPlatform(OrbDropPedestal ped){
		return destObj == ped;
	}

	public bool AtStart(){
		return startPos == transform.position;
	}

	public bool IsApproachingPlayer(){
			return initialApproach;
	}
}

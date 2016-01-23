using UnityEngine;
using System.Collections;

public class OrbDropPedestal : ActiveInteractable {
	//suppress warnings
	#pragma warning disable 414

	public Activatable[] triggers;//Activatable objects which this switch triggers

	Vector3 orbPos = new Vector3(0,.2f,0);
	Orb placedOrb = null;
	bool actionsTriggered = false;

	public void Start(){
		base.StartSetup();
		range = 2f;
	}

	public void FixedUpdate(){
		base.FixedUpdateLogic();
		CheckForActionTrigger();
	}

	private void CheckForActionTrigger(){
		if(HasOrb() && placedOrb.OnFinalPlatform() && !actionsTriggered){
			actionsTriggered = true;
			foreach(Activatable o in triggers)
				o.setActivated(true);
		}
	}

	private void PlaceOrb(){
		placedOrb = PlayerController.instance.getOrb();
		placedOrb.SetPlatform(this);
		PlayerController.instance.DropOrb();
	}

	private bool HasOrb(){
		return placedOrb != null;
	}

	//inherited
	public override void Triggered(){
		PlaceOrb();
	}

	protected override bool IsEnabled(){
		return PlayerController.instance.isHoldingOrb() && !HasOrb();
	}

	public override float GetDistance() {
		if (GameStateManager.is3D())
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
			                        new Vector2(transform.position.x, transform.position.y));
	}

	//public interface
	public Vector3 GetOrbPosition(){
		return transform.position + orbPos;
	}
}

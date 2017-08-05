using UnityEngine;
using System.Collections;

public class OrbSpawnPedestal : ActiveInteractable {
	public Orb myOrb;

	public override void Triggered(){
		moveOrbOutward();
		dropOrb();
	}

	private void moveOrbOutward(){
		float randAngle = Random.value * 2 * Mathf.PI;
		Vector2 outVec = new Vector2(
			Mathf.Cos(randAngle), Mathf.Sin(randAngle));
		myOrb.SetOutwardDropVector(outVec);
	}

	private void dropOrb(){
		if(PlayerController.instance.isHoldingOrb(myOrb)){
			PlayerController.instance.DropOrb();
		}else{
			myOrb.Drop();
			myOrb.SetVisible(false);
		    myOrb.breakSFX.Play();
		}
	}

	protected override bool IsEnabled(){
		return !myOrb.AtStart() && !myOrb.IsApproachingPlayer();
	}
}

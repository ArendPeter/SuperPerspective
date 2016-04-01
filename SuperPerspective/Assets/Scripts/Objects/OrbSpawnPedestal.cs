using UnityEngine;
using System.Collections;

public class OrbSpawnPedestal : ActiveInteractable {
	public Orb myOrb;

	public override void Triggered(){
		float randAngle = Random.value * 2 * Mathf.PI;
		Vector2 outVec = new Vector2(
			Mathf.Cos(randAngle), Mathf.Sin(randAngle));
		myOrb.SetOutwardDropVector(outVec);
		if(PlayerController.instance.isHoldingOrb(myOrb)){
			PlayerController.instance.DropOrb();
		}else{
			myOrb.Drop();
		}
	}

	protected override bool IsEnabled(){
		return !myOrb.AtStart() && !myOrb.IsApproachingPlayer();
	}
}

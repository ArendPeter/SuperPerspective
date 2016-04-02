using UnityEngine;
using System.Collections;

public class Pixie : ActiveInteractable {
	public convoNode myNode;

	public override void Triggered(){
		textBoxScript.instance.startConvo(myNode);
	}

	protected override bool IsEnabled(){
			return !textBoxScript.instance.IsEnabled();
	}

	protected override bool IsInYRange(){
		float myY = transform.position.y;
		float playerY = PlayerController.instance.transform.position.y;
		float diff = playerY - myY;
		return -3f < diff && diff < 1.5f;
	}
}

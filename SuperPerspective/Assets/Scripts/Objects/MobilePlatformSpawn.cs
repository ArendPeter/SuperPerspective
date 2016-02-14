using UnityEngine;
using System.Collections;

public class MobilePlatformSpawn : ActiveInteractable {

	public GameObject platform = null;

	float animSpeed = 0f;
	
	void Start() {
		base.StartSetup ();
		range = 1f;
	}

	void FixedUpdate() {
		base.FixedUpdateLogic();
		platform.transform.Translate(Vector3.up * animSpeed);
		float ydiff = platform.GetComponent<MobilePlatform>().GetStartY() - platform.transform.position.y;
		if (ydiff < 0.05) {
			animSpeed = 0;
			platform.transform.Translate(Vector3.up * ydiff);
		} else if (ydiff < 1) {
			animSpeed = 1/20f * ydiff * 2;
		}
	}

	public override float GetDistance() {
		if (player.transform.position.y > GetComponent<Collider>().bounds.max.y + 2)
			return float.MaxValue;
		float colMinX = GetComponent<Collider>().bounds.min.x;
		float colMaxX = GetComponent<Collider>().bounds.max.x;
		float colMinZ = GetComponent<Collider>().bounds.min.z;
		float colMaxZ = GetComponent<Collider>().bounds.max.z;
		switch (GetQuadrant ()) {
		case Quadrant.xPlus:
			return player.transform.position.x - colMaxX;
		case Quadrant.xMinus:
			return colMinX - player.transform.position.x;
		case Quadrant.zPlus:
			return player.transform.position.z - colMaxZ;
		case Quadrant.zMinus:
			return colMinZ - player.transform.position.z;
		default:
			return float.MaxValue;
		}
	}

	public override void Triggered() {
		if (animSpeed == 0) {
			platform.transform.position = transform.position;
			animSpeed = 1/10f;
		}
	}

}

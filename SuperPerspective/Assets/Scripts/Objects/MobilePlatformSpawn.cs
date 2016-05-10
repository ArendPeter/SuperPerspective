using UnityEngine;
using System.Collections;

public class MobilePlatformSpawn : ActiveInteractable {

	public GameObject platform = null;

	Vector3 spawnVector = new Vector3(0,-1.5f,0);

	float animSpeed = 0f;

	void Update() {
		if (player.transform.position.y - 0.5f < spawnVector.y) {
			range = 1f;
		} else {
			range = GetComponent<Collider>().bounds.size.y + 1f;
		}
	}

	void FixedUpdate() {
		base.FixedUpdateLogic();
		platform.transform.Translate(Vector3.up * animSpeed);
		float ydiff = Vector3.zero.y - platform.transform.localPosition.y;
		if (ydiff < 0.05) {
			animSpeed = 0;
			//platform.transform.Translate(Vector3.up * ydiff);
		} else if (ydiff < 1) {
			animSpeed = 1/20f * ydiff * 1.2f;
		}
	}

	public override float GetDistance() {
		Vector3 pPos = player.transform.position;
		Bounds bounds = GetComponent<Collider>().bounds;
		if (pPos.x >= bounds.min.x && pPos.x <= bounds.max.x && pPos.z >= bounds.min.z && pPos.z <= bounds.max.z) {
			return pPos.y - transform.position.y;
		}
		if (pPos.y > GetComponent<Collider>().bounds.max.y + 2)
			return float.MaxValue;
		float colMinX = bounds.min.x;
		float colMaxX = bounds.max.x;
		float colMinZ = bounds.min.z;
		float colMaxZ = bounds.max.z;
		switch (GetQuadrant ()) {
		case Quadrant.xPlus:
			return pPos.x - colMaxX;
		case Quadrant.xMinus:
			return colMinX - player.transform.position.x;
		case Quadrant.zPlus:
			return pPos.z - colMaxZ;
		case Quadrant.zMinus:
			return colMinZ - player.transform.position.z;
		default:
			return float.MaxValue;
		}
	}

	protected override bool isPlayerFacingObject() {
		Vector3 pPos = player.transform.position;
		Bounds bounds = GetComponent<Collider>().bounds;
		if (pPos.x >= bounds.min.x && pPos.x <= bounds.max.x && pPos.z >= bounds.min.z && pPos.z <= bounds.max.z) {
			return true;
		}
		return base.isPlayerFacingObject();
	}

	protected override bool IsInYRange(){
		float playerY = PlayerController.instance.transform.position.y;
		float myY = transform.position.y;
		float deltaY = playerY - myY;
		return -0.2f < deltaY && deltaY < 1f;
	}

	public override void Triggered() {
		if (animSpeed == 0) {
			platform.transform.localPosition = spawnVector;
			animSpeed = 1/10f;
		}
	}

}

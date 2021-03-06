﻿using UnityEngine;
using System.Collections;

public class BoundObject : MonoBehaviour {

	#pragma warning disable 414

	Rect myBounds;
	Rect[] bounds;
	//float altLeftBound = -1;//-1 means no alternate left bound
	//float altRightBound = -1;
	private float groundY = 0;
	private bool hasBound = false;
	private int boundIndex = -1;

	// Use this for initialization
	void Start () {
		bounds = IslandControl.instance.islandBounds;
		updateBounds();
	}

	public void updateBounds(){
		Vector3 pos = transform.position;
		boundIndex = IslandControl.instance.getBound (pos.x, pos.y, pos.z, !GameStateManager.is3D());
		if(boundIndex == -1)
				return;
		hasBound = true;
		//update myBounds
		float halfWidth = transform.lossyScale.x / 2f;
		float halfDepth = transform.lossyScale.z / 2f;
		myBounds = new Rect(
			bounds[boundIndex].xMin + halfWidth,
			bounds[boundIndex].yMin + halfDepth,
			bounds[boundIndex].width - (halfWidth * 2),
			bounds[boundIndex].height - (halfDepth *2)
		);

		//bind to new bounds
		bind ();
	}

	// Update is called once per frame
	void LateUpdate () {
		if(!hasBound){
			updateBounds();
		}
		if(hasBound){
			bind ();
		}
	}

	public void bind(){
		bool pMode = (GameStateManager.instance.currentPerspective == PerspectiveType.p3D);
		//find velocity
		PhysicalObject po = gameObject.GetComponent<PhysicalObject>();
		Vector3 vel = Vector3.zero;
		if(po!=null)
			vel = po.getVelocity();
		//bind by position
		Vector3 startPos = transform.position;
		Vector3 pos = startPos;
		//bind along x
		pos.x = Mathf.Max (myBounds.xMin, Mathf.Min (myBounds.xMax, pos.x));
		//bind along z
		if(pMode)
			pos.z = Mathf.Max (myBounds.yMin, Mathf.Min (myBounds.yMax, pos.z));
		//reset velocity if necessary
		if(startPos.x != pos.x)
			vel.x = 0;
		if(startPos.z != pos.z)
			vel.z = 0;
		//update variables
		if(po!=null)
			po.setVelocity(vel);
		transform.position = pos;
	}

	void bindAlternate(){
		Vector3 pos = transform.position;
		pos.x = Mathf.Max (myBounds.xMin, pos.x);
		pos.x = Mathf.Min (myBounds.xMax, pos.x);
		if(GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			pos.z = Mathf.Max (myBounds.yMin, Mathf.Min (myBounds.yMax, transform.position.z));
		transform.position = pos;
	}

	public Rect GetBounds() {
		return myBounds;
	}

	public int GetBoundIndex() {
		return boundIndex;
	}
}

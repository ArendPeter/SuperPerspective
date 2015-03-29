﻿using UnityEngine;
using System.Collections;

public class CollisionChecker {

	Collider col;
	float colliderWidth, colliderHeight, colliderDepth;

	public CollisionChecker(Collider col) {
		this.col = col;
		colliderWidth = col.bounds.max.x - col.bounds.min.x;
		colliderHeight = col.bounds.max.y - col.bounds.min.y;
		colliderDepth = col.bounds.max.z - col.bounds.min.z;
	}
	
	public RaycastHit CheckXCollision(Vector3 velocity, float Margin) {
		colliderWidth = col.bounds.max.x - col.bounds.min.x;
		Vector3[] startPoints = new Vector3[5];
		RaycastHit hitInfo = new RaycastHit();

		float minX 		= col.bounds.min.x + Margin;
		float centerX 	= col.bounds.center.x;
		float maxX 		= col.bounds.max.x - Margin;
		float minY 		= col.bounds.min.y + Margin;
		float centerY 	= col.bounds.center.y;
		float maxY 		= col.bounds.max.y - Margin;
		float minZ 		= col.bounds.min.z + Margin;
		float centerZ	= col.bounds.center.z;
		float maxZ 		= col.bounds.max.z - Margin;

		// True if any ray hits a collider
		bool connected = false;
		
		// Set the raycast distance to check as far as the player will move this frame
		float distance = (colliderWidth / 2) + Mathf.Abs(velocity.x * Time.deltaTime);
		
		//setup startPoints, note /**/ means margin wasn't applied previously
		startPoints[0] = new Vector3(centerX, minY, maxZ/**/);
		startPoints[1] = new Vector3(centerX, maxY/**/, maxZ/**/);
		startPoints[2] = new Vector3(centerX, minY, minZ/**/);
		startPoints[3] = new Vector3(centerX, maxY, minZ);
		startPoints[4] = new Vector3(centerX, centerY, centerZ);
		
		//test all startpoints
		Vector3 dir = Vector3.right * Mathf.Sign(velocity.x);
		for(int i = 0; i < startPoints.Length && !connected; i++)
			connected = Physics.Raycast(startPoints[i], dir, out hitInfo, distance);
		
		return hitInfo;
	}

	public RaycastHit CheckYCollision(Vector3 velocity, float Margin) {
		colliderHeight = col.bounds.max.y - col.bounds.min.y;
		RaycastHit hitInfo = new RaycastHit();
		
		float minX 		= col.bounds.min.x + Margin;
		float centerX 	= col.bounds.center.x;
		float maxX 		= col.bounds.max.x - Margin;
		float minY 		= col.bounds.min.y + Margin;
		float centerY 	= col.bounds.center.y;
		float maxY 		= col.bounds.max.y - Margin;
		float minZ 		= col.bounds.min.z + Margin;
		float centerZ	= col.bounds.center.z;
		float maxZ 		= col.bounds.max.z - Margin;
		
		// True if any ray hits a collider
		bool connected = false;
		
		// Set the raycast distance to check as far as the player will fall this frame
		float distance = (colliderHeight / 2) + Mathf.Abs(velocity.y * Time.deltaTime);
		
		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, centerY, maxZ),
			new Vector3(maxX, centerY, maxZ),
			new Vector3(minX, centerY, minZ),
			new Vector3(maxX, centerY, minZ),
			new Vector3(centerX, centerY, centerZ)
		};
		
		//test all startpoints
		Vector3 dir = Vector3.up * Mathf.Sign(velocity.y);
		// must run outside loop once to ensure hitInfo is initialized
		connected = Physics.Raycast(startPoints[0],dir, out hitInfo, distance);
		for(int i = 1; i < startPoints.Length && !connected; i++)
			connected = Physics.Raycast(startPoints[i],dir, out hitInfo, distance);
		
		return hitInfo;
	}

	public RaycastHit CheckZCollision(Vector3 velocity, float Margin) {
		colliderDepth = col.bounds.max.z - col.bounds.min.z;
		Vector3[] startPoints = new Vector3[5];
		RaycastHit hitInfo = new RaycastHit();
		
		float minX 		= col.bounds.min.x + Margin;
		float centerX 	= col.bounds.center.x;
		float maxX 		= col.bounds.max.x - Margin;
		float minY 		= col.bounds.min.y + Margin;
		float centerY 	= col.bounds.center.y;
		float maxY 		= col.bounds.max.y - Margin;
		float minZ 		= col.bounds.min.z + Margin;
		float centerZ	= col.bounds.center.z;
		float maxZ 		= col.bounds.max.z - Margin;

		bool connected = false;

		// Set the raycast distance to check as far as the player will move this frame
		float distance = (colliderDepth / 2 + Mathf.Abs(velocity.z * Time.deltaTime));
		
		//setup startPoints arary
		startPoints[0] = new Vector3(minX, maxY, centerZ);
		startPoints[1] = new Vector3(maxX, maxY, centerZ);
		startPoints[2] = new Vector3(minX, minY, centerZ);
		startPoints[3] = new Vector3(maxX, minY, centerZ);
		startPoints[4] = new Vector3(centerX, centerY, centerZ);
		
		//loop through and check all startpoints
		Vector3 dir = Vector3.forward * Mathf.Sign(velocity.z);
		for(int i = 0; i < startPoints.Length && !connected; i++)
			connected = Physics.Raycast(startPoints[i], dir, out hitInfo, distance);

		return hitInfo;
	}
}

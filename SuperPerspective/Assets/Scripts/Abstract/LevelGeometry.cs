﻿using UnityEngine;
using System.Collections;

/// <summary>
///     Attach this script to any level geometry that must be collidable in 2D perspective.
///     Stretches the collider of the geometry to match the Z depth of the platform the geometry is part of.
/// </summary>
public class LevelGeometry : MonoBehaviour
{

	#region Properties & Variables

	private GameObject parentPlatform;   // The platform this geometry belongs to

	private float platWidth;
	public Vector3 offset = Vector3.zero;

	private BoxCollider boxCollider;    // Reference to this object's BoxCollider
	private Vector3 colliderSize;       // Stores the collider's beginning size, usually (1, 1, 1)
	private Vector3 startCenter;
	private Quaternion startRotation;
	private float zDiff;

	#endregion Properties & Variables


	#region Monobehavior Implementation	
	
	void Start () {
		parentPlatform = IslandControl.instance.findGround(this.gameObject);
		
      // Register to perspective shift event
		boxCollider = GetComponent<BoxCollider>();
		startCenter = boxCollider.center;
		colliderSize = boxCollider.size;

		platWidth = parentPlatform.GetComponent<Collider> ().bounds.size.z;

		GameStateManager.instance.PerspectiveShiftEvent += AdjustPosition;

		AdjustPosition(GameStateManager.instance.currentPerspective);
	}

    #endregion Monobehavior Implementation


    #region Perspective Shift Event

    
    // Adjusts the collider to the appropriate shape when the perspective shift event occurs.
	public void AdjustPosition(PerspectiveType p)
    {
		float rot = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(Vector3.forward, transform.forward));
		if (Mathf.Round(rot) == 90 && Mathf.Round(Vector3.Angle(transform.right, Vector3.forward)) == 0)
			rot = 270;
		if (p == PerspectiveType.p2D && parentPlatform!=null)
		{
			boxCollider.size = new Vector3(
				colliderSize.x * Mathf.Cos(rot * Mathf.Deg2Rad) + (platWidth / transform.lossyScale.x) * Mathf.Sin(rot * Mathf.Deg2Rad), 
				colliderSize.y, 
				colliderSize.z * Mathf.Sin(rot * Mathf.Deg2Rad) + (platWidth / transform.lossyScale.z) * Mathf.Cos(rot * Mathf.Deg2Rad));
			if (Mathf.Round(rot) == 90 || Mathf.Round(rot) == 270)
				boxCollider.center = new Vector3(-(parentPlatform.transform.position.z - transform.position.z) * (1 / Mathf.Abs(transform.localScale.x)) * Mathf.Sin(rot * Mathf.Deg2Rad), startCenter.y, startCenter.z);
			else
				boxCollider.center = new Vector3(startCenter.x + offset.x, startCenter.y + offset.y, (parentPlatform.transform.position.z - transform.position.z) * (1 / Mathf.Abs(transform.localScale.z)) * Mathf.Cos(rot * Mathf.Deg2Rad) + offset.z);
		}
        else if (p == PerspectiveType.p3D)
        {
			boxCollider.center = startCenter;
			boxCollider.size = colliderSize;
        }
    }
	 
	public Vector3 getTrueBoxColliderCenter(){
		return startCenter;
	}

	public Vector3 getTrueBoxColliderSize(){
		return colliderSize;
	}

    #endregion Perspective Shift Event
}

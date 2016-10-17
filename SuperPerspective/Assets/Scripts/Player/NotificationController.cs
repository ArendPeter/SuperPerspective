using UnityEngine;
using System.Collections;

public class NotificationController : MonoBehaviour {
	
	//GameObjects representing the different planes for the notification
	Renderer plane3D, plane2D, plane3DX, plane2DX;
	//whether or not it's visible
	bool visible = false;
	
	//find planes
	void Start(){
		plane3D = transform.GetChild(0).GetComponent<Renderer>();
		plane2D = transform.GetChild(1).GetComponent<Renderer>();
        plane3DX = transform.GetChild(2).GetComponent<Renderer>();
        plane2DX = transform.GetChild(3).GetComponent<Renderer>();
    }
	
	//update visibility of planes
	void FixedUpdate () {
		plane3D.enabled = visible &&  GameStateManager.is3D();
		plane2D.enabled = visible && !GameStateManager.is3D();
        plane3DX.enabled = visible && GameStateManager.is3D();
        plane2DX.enabled = visible && !GameStateManager.is3D();
    }
	
	public void updateVisible(bool visible){
		this.visible = visible;
	}
}

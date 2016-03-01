using UnityEngine;
using System.Collections;

public class PushSwitchOld : MonoBehaviour {

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool pushed = false; //whether switch is currently pushed

	Color baseColor = Color.white;

    public Rect parentPlatform;
	Collider pusher = null;

	void Update(){
		parentPlatform = PlayerController.instance.GetComponent<BoundObject>().GetBounds();
        float xx = transform.position.x, yy = transform.position.y, zz = transform.position.z, cdist, r = 0.25f, w = 1f;
		Collider c = GetComponent<Collider>();
		Vector3 p1, p2;
		if (GameStateManager.is2D()) {
			p1 = new Vector3(xx + w - r, yy + r, parentPlatform.min.y + r);
			p2 = new Vector3(xx - w + r, yy + r, parentPlatform.min.y + r);
			cdist = parentPlatform.height - r * 2;
		} else {
			p1 = new Vector3(xx + w - r, yy + r, zz - w + r);
			p2 = new Vector3(xx - w + r, yy + r, zz - w + r);
			cdist = w - r * 2;
		}
		RaycastHit[] collisions = Physics.CapsuleCastAll(p1, p2, r, Vector3.forward, cdist, LayerMask.NameToLayer("Raycast Ignore"));
		if (collisions.Length == 0)
			ExitCollisionWithGeneral(null);
		else {
			foreach (RaycastHit hit in collisions) {
				GameObject obj = hit.collider.gameObject;
				bool isPusher = (obj.GetComponent<Ice>() != null) || (obj.GetComponent<Crate>() != null) || (obj.GetComponent<PlayerController>() != null);
				if (!pushed && isPusher)
					EnterCollisionWithGeneral(obj);
			}
		}
        if (pushed) {
            if (baseColor == Color.white)
                baseColor = gameObject.GetComponent<Renderer>().material.color;
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        } else {
            if (baseColor != Color.white)
                gameObject.GetComponent<Renderer>().material.color = baseColor;
        }
    }

    public void EnterCollisionWithGeneral(GameObject other){
		pushed = true;//becomes pushed when it collides with player
		//pusher = other.GetComponent<Collider> ();
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}

	public void ExitCollisionWithGeneral(GameObject other){
		pushed = false;//becomes pushed when it collides with player
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
}

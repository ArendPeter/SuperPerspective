using UnityEngine;
using System.Collections;

public class PushSwitchOld : MonoBehaviour {

	public Activatable[] triggers;//Activatable objects which this switch triggers

	bool pushed = false; //whether switch is currently pushed

	Color baseColor = Color.white;

	ArrayList collisions;

    public Rect parentPlatform;
	Collider pusher = null;

    public GameObject p1, p2;

	void Start() {
		collisions = new ArrayList();
	}

	void Update(){
		parentPlatform = PlayerController.instance.GetComponent<BoundObject>().GetBounds();
        float xx = transform.position.x, yy = transform.position.y, zz = transform.position.z, cdist, r, wx, wz;
		wx = GetComponent<Renderer>().bounds.size.x / 4;
		wz = GetComponent<Renderer>().bounds.size.z / 4;
		r = 0.25f;
		Collider c = GetComponent<Collider>();
		Vector3 p1, p2;
		if (GameStateManager.is2D()) {
			p1 = new Vector3(xx + wx - r, yy + r, parentPlatform.min.y + r);
			p2 = new Vector3(xx - wx + r, yy + r, parentPlatform.min.y + r);
			cdist = parentPlatform.height - r * 2;
		} else {
			p1 = new Vector3(xx + wx - r, yy + r, zz - wz + r);
			p2 = new Vector3(xx - wx + r, yy + r, zz - wz + r);
			cdist = wz * 2 - r * 2;
		}
		foreach (RaycastHit hit in Physics.CapsuleCastAll(p1, p2, r, Vector3.forward, cdist, 7)) {
			if (hit.collider.gameObject.GetComponent<Rigidbody>() == null)
				collisions.Add(hit);
		}
		if (collisions.Count == 0)
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
		collisions.Clear();
    }

    public void EnterCollisionWithGeneral(GameObject other){
		pushed = true;//becomes pushed when it collides with player
        p1.SetActive(false);
        p2.SetActive(false);
        //pusher = other.GetComponent<Collider> ();
        //pushed is also updated for all activatable objects
        foreach (Activatable o in triggers)
			o.setActivated(pushed);
	}

	public void ExitCollisionWithGeneral(GameObject other){
		pushed = false;//becomes pushed when it collides with player
        p1.SetActive(true);
        p2.SetActive(true);
        //pushed is also updated for all activatable objects
        foreach (Activatable o in triggers)
			o.setActivated(pushed);
	}
}

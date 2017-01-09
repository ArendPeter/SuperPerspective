using UnityEngine;
using System.Collections;

public class Teleporter2D : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll != null)
        {
            CharacterController2D temp = coll.gameObject.GetComponent<CharacterController2D>();
            if (temp != null && temp.grounded)
            {
                temp.StartTeleport();
            }
        }
    }

}

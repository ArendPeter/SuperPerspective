using UnityEngine;
using System.Collections;

public class Orb2D : MonoBehaviour {

    public Transform target;
    public float speed = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, speed);
    }
}

using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public float xSpeed, ySpeed, zSpeed;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(xSpeed*Time.deltaTime, ySpeed*Time.deltaTime, zSpeed*Time.deltaTime));
	}
}

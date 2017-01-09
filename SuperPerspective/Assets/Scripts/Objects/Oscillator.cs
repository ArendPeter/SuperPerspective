using UnityEngine;
using System.Collections;

public class Oscillator : MonoBehaviour {

    Vector3 initPos, curPos;
    public float strength, speed;
    public bool vertical;

	// Use this for initialization
	void Start () {
        initPos = gameObject.transform.position;
        curPos = new Vector3(initPos.x, initPos.y, initPos.z);
    }
	
	// Update is called once per frame
	void Update () {
        if (vertical)
        {
            gameObject.transform.position = new Vector3(initPos.x, initPos.y + strength * Mathf.Sin(Time.time * speed), initPos.z);
        }
        else
        {
            gameObject.transform.position = new Vector3(initPos.x + strength * Mathf.Sin(Time.time * speed), initPos.y, initPos.z);
        }
	}
}

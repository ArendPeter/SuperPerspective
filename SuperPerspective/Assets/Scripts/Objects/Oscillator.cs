using UnityEngine;
using System.Collections;

public class Oscillator : MonoBehaviour {

    Vector3 initPos, curPos;
    public float strength, time;
    float curTime;

	// Use this for initialization
	void Start () {
        initPos = gameObject.transform.position;
        strength = 0.25f;
        time = 4;
        curTime = time;
        curPos = new Vector3(initPos.x, initPos.y, initPos.z);
    }
	
	// Update is called once per frame
	void Update () {
	    if (curTime <= 0)
        {
            curTime = time;
        }
        else
        {
            gameObject.transform.position = new Vector3(initPos.x, initPos.y + strength*Mathf.Sin(Time.time), initPos.z);
        }
	}
}

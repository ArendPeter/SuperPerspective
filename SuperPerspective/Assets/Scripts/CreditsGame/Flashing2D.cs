using UnityEngine;
using System.Collections;

public class Flashing2D : MonoBehaviour {

    public float maxTimer = 1;
    float timer;
    SpriteRenderer r;

	// Use this for initialization
	void Start () {
        timer = maxTimer;
        r = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            r.enabled = !r.enabled;
            timer = maxTimer;
        }
	}
}

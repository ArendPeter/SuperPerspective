using UnityEngine;
using System.Collections;

public class FairyFollow2D : MonoBehaviour {

    public Transform target, player;
    public float speed = 0.5f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x > player.transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
	}

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, speed);
    }

}

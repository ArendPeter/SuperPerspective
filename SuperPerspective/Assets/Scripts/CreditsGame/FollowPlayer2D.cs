using UnityEngine;
using System.Collections;

public class FollowPlayer2D : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("HoodieGirl2D");
	}
	
	// Update is called once per frame
	void Update () {
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
	}
}

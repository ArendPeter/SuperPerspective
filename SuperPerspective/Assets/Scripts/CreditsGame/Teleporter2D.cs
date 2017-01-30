using UnityEngine;
using System.Collections;

public class Teleporter2D : MonoBehaviour {

    public Transform target;
    private float delay = 3;
    bool hasEntered = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll != null && !hasEntered)
        {
            CharacterController2D temp = coll.gameObject.GetComponent<CharacterController2D>();
            if (temp != null && temp.grounded)
            {
                hasEntered = true;
                ProgressManager2D.instance.levelProgress++;
                MessageManager2D.instance.levelScore.text = "Levels Completed: " + ProgressManager2D.instance.levelProgress;
                temp.StartTeleport();
                if (target != null)
                {
                    StartCoroutine(temp.EndTeleport(target.position, delay));
                }
            }
        }
    }

}

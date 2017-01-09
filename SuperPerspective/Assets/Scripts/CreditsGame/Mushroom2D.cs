using UnityEngine;
using System.Collections;

public class Mushroom2D : MonoBehaviour {

    public float force = 7.5f;

    private float t, damp;
    private bool animating;
    private Vector3 startScale;

    public Transform mushroom;

    // Use this for initialization
    void Start () {
        startScale = mushroom.localScale;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll != null)
        {
            CharacterController2D temp = coll.gameObject.GetComponent<CharacterController2D>();
            if (temp != null)
            {
                temp.rb.velocity = (new Vector3(temp.rb.velocity.x, force));
                animating = true;
                damp = 1;
            }
        }
    }

    void FixedUpdate()
    {
        if (animating)
        {
            Vector3 newScale = mushroom.localScale;
            newScale.x = startScale.x + (startScale.x * 0.2f) * Mathf.Sin(t) * damp;
            newScale.y = startScale.y - (startScale.y * 0.2f) * Mathf.Sin(t) * damp;
            mushroom.localScale = newScale;
            t += Mathf.PI / 8;
            if (t >= Mathf.PI * 2)
            {
                if (damp < 0.1)
                {
                    animating = false;
                    mushroom.localScale = startScale;
                }
                else
                    damp /= 2;
                t = 0;
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class Ice2D : Interactible2D {

    public IsColliding2D left, right;
    private Rigidbody2D rb;
    public float speed = 5;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (rb.velocity.x > 0)
        {
            if (right.isColliding)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }
        }
        else if (rb.velocity.x < 0)
        {
            if (left.isColliding)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }
        }
    }

    public override void Activate(CharacterController2D playerPos)
    {
        if (playerPos.transform.position.x < transform.position.x && !right.isColliding)
        {
            rb.velocity = new Vector2(speed, 0);
            rb.isKinematic = false;
        }
        else if (playerPos.transform.position.x >= transform.position.x && !left.isColliding)
        {
            rb.velocity = new Vector2(-speed, 0);
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
        }
    }

}

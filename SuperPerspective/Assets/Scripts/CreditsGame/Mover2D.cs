using UnityEngine;
using System.Collections;

public class Mover2D : MonoBehaviour {

    public bool activated;
    public Vector3 movement;
    public float transitionSpeed = 0.05f;
    private Vector3 startPos;
    private float prog;

	// Use this for initialization
	void Start () {
        prog = 0;
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        prog += (transitionSpeed) * ((activated) ? 1 : -1);
        prog = Mathf.Clamp01(prog);
        transform.position = Vector3.Lerp(startPos, startPos + movement, prog);
    }

    //public override void Activate(CharacterController2D player)
    //{
        
    //}

}

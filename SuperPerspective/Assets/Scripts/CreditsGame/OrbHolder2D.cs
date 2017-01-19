using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbHolder2D : Interactible2D {

    public bool isStart = true;
    public Orb2D orb;
    public Transform orbHolder;
    public Mover2D mover;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Activate(CharacterController2D player)
    {
        //take orb
        if (orb != null && player.orb == null)
        {
            orb.target = player.orbMount;
            player.orb = orb;
            orb = null;
            if (mover != null)
            {
                mover.activated = false;
            }
        }
        //place orb
        else if (player.orb != null && orb == null)
        {
            orb = player.orb;
            player.orb = null;
            orb.target = orbHolder;
            if (mover != null)
            {
                mover.activated = true;
            }
        }
    }

}

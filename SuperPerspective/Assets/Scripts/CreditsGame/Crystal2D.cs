﻿using UnityEngine;
using System.Collections;

public class Crystal2D : MonoBehaviour {

    public ParticleSystem ambient, poof;

	// Use this for initialization
	void Start () {
	
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
                if (!Fadeout2D.instance.totallyDone)
                {
                    ProgressManager2D.instance.crystalProgress++;
                    MessageManager2D.instance.crystalScore.text = "Crystals Collected: " + ProgressManager2D.instance.crystalProgress + "/3";
                }
                GetComponent<SpriteRenderer>().enabled = false;
                ambient.Stop();
                poof.Play();
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
}

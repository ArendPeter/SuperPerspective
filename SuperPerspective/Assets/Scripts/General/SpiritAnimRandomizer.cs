using UnityEngine;
using System.Collections;

public class SpiritAnimRandomizer : MonoBehaviour {

    private Animator anim;
    public int currentInt = 0;

	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void randomize()
    {
        currentInt = Random.Range(0, 3);
        anim.SetInteger("RandTransition", currentInt);
    }

    public void teach()
    {
        anim.SetTrigger("Teach");
    }


}

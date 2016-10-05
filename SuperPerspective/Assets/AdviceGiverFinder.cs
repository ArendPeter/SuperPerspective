using UnityEngine;
using System.Collections;

public class AdviceGiverFinder : MonoBehaviour {

    public AdviceGiver ad;

	// Use this for initialization
	void Start () {
        ad = GameObject.FindObjectOfType<AdviceGiver>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

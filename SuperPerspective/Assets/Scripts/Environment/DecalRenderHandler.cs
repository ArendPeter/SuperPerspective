using UnityEngine;
using System.Collections;

//this handles disabling rendering to improve performance
public class DecalRenderHandler : MonoBehaviour {

	public float renderDist = 50.0f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 playerDist = PlayerController.instance.transform.position;
		Vector3 thisDist = this.transform.position;
		if(Vector3.Distance(thisDist, playerDist) <= renderDist) {
			setVisible();
		}else{
			setInvisible();
		}
	}

	void setVisible() {
		foreach (Transform child in transform)
		{
    		child.gameObject.SetActive(true);
		}
    }

    void setInvisible () {
    	foreach (Transform child in transform)
		{
    		child.gameObject.SetActive(false);
		}
    }

}

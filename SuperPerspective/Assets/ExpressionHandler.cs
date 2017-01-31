using UnityEngine;
using System.Collections;

public class ExpressionHandler : MonoBehaviour {

    public GameObject mouthBone;

    public float animSpeed;

    public float   closedAngle;
    public float   openAngle;

    public float currentAngle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        boneLerp();
	}

    public void expressJoy()
    {
        mouthBone.transform.rotation = Quaternion.Euler(openAngle, mouthBone.transform.rotation.y, mouthBone.transform.rotation.z);
        currentAngle = openAngle;
    }

    public void boneLerp()
    {
        
        currentAngle = Mathf.Lerp(currentAngle, closedAngle, animSpeed);
        //print(currentAngle);
        mouthBone.transform.rotation = Quaternion.Euler(currentAngle, mouthBone.transform.rotation.y, mouthBone.transform.rotation.z);
    }

}

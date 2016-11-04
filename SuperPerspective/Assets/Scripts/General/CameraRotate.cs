using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour {

	public float test;

	public Material[] skys;

	public float[] trans;
	public float transitionRange;

    public GameObject[] seats;

	public float rotateSpeed, radius, height;

	private float angle = 0;

	void Start(){
		RenderSettings.skybox = skys[0];
	}

	void Update () {
		updateRotation();
		updateSkybox();
	}

	private void updateRotation(){
		//position
		angle +=	rotateSpeed * Time.deltaTime;
		angle %= 2 * Mathf.PI;
		Vector3 pos = new Vector3(
			radius * Mathf.Cos(angle),
			height,
			radius * Mathf.Sin(angle));
		transform.position = pos;

		//rotation
		transform.rotation = Quaternion.LookRotation(-transform.position, Vector3.up );
	}

	private void updateSkybox(){
		int curIndex = 0, nextIndex;
		while(curIndex < 3 && angle > trans[curIndex]){
			curIndex++;
		}
		curIndex = curIndex%3;
		nextIndex = (curIndex+1)%3;
		float t = 0;
		float distToTrans = trans[nextIndex] - angle;
		if(distToTrans < 0){ distToTrans += Mathf.PI * 2; }
		if(distToTrans < transitionRange){
			t = distToTrans / transitionRange;
		}
		//skys[3].Lerp(skys[0],skys[1],test);
		RenderSettings.skybox = skys[curIndex];
        disableAllSeats();
        seats[curIndex].SetActive(true);
    }

    public void disableAllSeats()
    {
        for (int i = 0; i < seats.Length; i++)
        {
            seats[i].SetActive(false);
        }
    }
}

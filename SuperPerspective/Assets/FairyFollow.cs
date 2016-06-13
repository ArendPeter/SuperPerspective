using UnityEngine;
using System.Collections;

public class FairyFollow : MonoBehaviour {

    //PlayerController playerC;
    GameObject player;
    public float followSpeed = .5f;
    public float floatHeight = 1f;
    public int arrayID = 0;

    public GameObject[] floatPoints = new GameObject[3];

	// Use this for initialization
	void Start () {
        //playerC = GameObject.FindObjectOfType<PlayerController>();
        player = GameObject.Find("HoodieAnim1");
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPos = new Vector3(floatPoints[arrayID].transform.position.x, floatPoints[arrayID].transform.position.y + floatHeight, floatPoints[arrayID].transform.position.z);
        transform.position = Vector3.Lerp(transform.position, playerPos, followSpeed);
        Vector3 vec = Vector3.Lerp(transform.localRotation.eulerAngles, player.transform.localRotation.eulerAngles, followSpeed);
        transform.rotation = Quaternion.Euler(vec.x, vec.y, vec.z);
    }
}

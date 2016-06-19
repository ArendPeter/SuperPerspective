using UnityEngine;
using System.Collections;

public class FairyFollow : MonoBehaviour {

    //PlayerController playerC;
    GameObject player;
    public float followSpeed = .5f;
    public float floatHeight = 1f;
    public int arrayID = 0;

    public float rayLength = 10f;

    public SpiritMount mount;

    private GameObject[] floatPoints = new GameObject[3];
    [SerializeField] private bool[] boolArr = new bool[3];

	// Use this for initialization
	void Start () {
        init();
    }
	
	// Update is called once per frame
	void Update () {
        if (mount != null)
        {
            posCheck();
            Vector3 playerPos = new Vector3(floatPoints[arrayID].transform.position.x, floatPoints[arrayID].transform.position.y + floatHeight, floatPoints[arrayID].transform.position.z);
            transform.position = Vector3.Lerp(transform.position, playerPos, followSpeed);
            Vector3 vec = Vector3.Lerp(transform.localRotation.eulerAngles, player.transform.localRotation.eulerAngles, followSpeed);
            transform.rotation = Quaternion.Euler(vec.x, vec.y, vec.z);
        }
        else
        {
            init();
            mount = GameObject.FindObjectOfType<SpiritMount>();
        }
    }

    public void posCheck()
    {
        for(int i = 0; i < floatPoints.Length; i++)
        {
            Vector3 up = transform.TransformDirection(Vector3.up) * rayLength;
            boolArr[i] = Physics.Raycast(floatPoints[i].transform.position, Vector3.up, rayLength);
            Debug.DrawRay(floatPoints[i].transform.position, up, Color.green);
        }

        if (boolArr[0])
        {
            if(Vector3.Distance(floatPoints[1].transform.position, Camera.main.transform.position) < Vector3.Distance(floatPoints[2].transform.position, Camera.main.transform.position))
            {
                arrayID = 1;
            }
            else
            {
                arrayID = 2;
            }
        }
        else
        {
            arrayID = 0;
        }

    }

    public void init()
    {
        //playerC = GameObject.FindObjectOfType<PlayerController>();
        mount = GameObject.FindObjectOfType<SpiritMount>();
        floatPoints[0] = mount.topFloatPoint;
        floatPoints[1] = mount.leftFloatPoint;
        floatPoints[2] = mount.rightFloatPoint;
        player = GameObject.Find("HoodieAnim1");
    }

}

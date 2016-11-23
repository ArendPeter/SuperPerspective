using UnityEngine;
using System.Collections;

public class FairyFollow : MonoBehaviour {

    //PlayerController playerC;
    GameObject player;
    public float followSpeed = .5f;
    public float floatHeight = 1f;
    public int arrayID = 0;

    [Tooltip("Used to determine if our Spirit is going to collide with something above us.")]
    public float rayLength = 10f;

    [Tooltip("This is where our spirit crystal floats toward. Essentially a location transform tagged by a specific class.")]
    public SpiritMount mount;

    public Animator anim;

    //The locations we can foat to.
    private GameObject[] floatPoints = new GameObject[3];
    //We use this to show which of the current locations we can use are currently free.
    [SerializeField] private bool[] boolArr = new bool[3];

    [Tooltip("Whether or not we should even be floating towards our target")]
    public bool shouldFollow = true;

	// Use this for initialization
	void Start () {
        init();
    }
	
	// Update is called once per frame
	void Update () {
        if (mount != null)
        {
            if (shouldFollow)
            {
                posCheck();
                Vector3 playerPos = new Vector3(floatPoints[arrayID].transform.position.x, floatPoints[arrayID].transform.position.y + floatHeight, floatPoints[arrayID].transform.position.z);
                transform.position = Vector3.Lerp(transform.position, playerPos, followSpeed);
                Vector3 vec = Vector3.Lerp(transform.localRotation.eulerAngles, player.transform.localRotation.eulerAngles, followSpeed);
                transform.rotation = Quaternion.Euler(vec.x, vec.y, vec.z);
            }
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
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        mount = GameObject.FindObjectOfType<SpiritMount>();
        floatPoints[0] = mount.topFloatPoint;
        floatPoints[1] = mount.leftFloatPoint;
        floatPoints[2] = mount.rightFloatPoint;
        //player = GameObject.Find("HoodieAnim1");
        anim = this.GetComponent<Animator>();
    }

    public void findPlayer()
    {
        if (!(player != null)){
            player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        }
        AdviceGiver ag = GameObject.FindObjectOfType<AdviceGiver>();
        ag.ivan = this;
    }

}

using UnityEngine;
using System.Collections;

public class ProgressManager2D : MonoBehaviour {

    public int levelProgress = 0;
    public int crystalProgress = 0;

    public static ProgressManager2D instance;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

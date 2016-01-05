using UnityEngine;
using System.Collections;

public class ListenerHandler : MonoBehaviour {

    bool is2D;
    float globZpos;

	// Use this for initialization
	void Start () {
        GameStateManager.instance.PerspectiveShiftSuccessEvent += SwapState;
        is2D = true;
        globZpos = transform.position.z;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(is2D);
        if (is2D)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, globZpos);
        }
    }

    void SwapState()
    {
        if (GameStateManager.instance.currentPerspective == PerspectiveType.p2D)
        {
            is2D = true;
            globZpos = transform.position.z;
        }

        else if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
        {
            is2D = false;
            transform.localPosition = Vector3.zero;
        }
    }
}

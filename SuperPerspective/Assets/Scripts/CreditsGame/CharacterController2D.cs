using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void DetectMovement()
    {
        float temp = InputManager.instance.GetForwardMovement();
        if (temp > 0)
        {
            Move(new Vector3(temp, 0, 0));
        }
        else if (temp < 0)
        {
            Move(new Vector3(temp, 0, 0));
        }
        else
        {

        }
    }

    void Move(Vector3 movement)
    {
        transform.Translate(movement*0.05f);
    }

    // Update is called once per frame
    void Update () {
        DetectMovement();
	}
}

using UnityEngine;
using System.Collections;

public class IsColliding2D : MonoBehaviour {
    
    public bool isColliding;

    public void Update()
    {
        //Debug.Log(isColliding);
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        isColliding = true;
    }
    void OnTriggerExit2D(Collider2D hit)
    {
        isColliding = false;
    }
}

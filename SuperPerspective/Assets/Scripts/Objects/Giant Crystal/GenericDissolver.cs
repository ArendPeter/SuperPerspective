using UnityEngine;
using System.Collections;

public class GenericDissolver : MonoBehaviour {

    public bool shouldDissolveObject = false;

    //FRIENDLY REMINDER THAT YOU HAVE TO HAVE A DISSOLVE SHADER ON THE OBJECT FOR THIS TO WORK.
    private Renderer dissolveRenderer;

    public float dissolveAmount = 0;
    public float dissolveSpeed = .05f;

    // Use this for initialization
    void Start () {
        dissolveRenderer = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (shouldDissolveObject)//Start Dissolving
        {

            //If we aren't done yet
            if (dissolveAmount <= 1)
            {
                //Keep going
                dissolveRenderer.material.SetFloat("_SliceAmount", dissolveAmount);
                dissolveAmount += dissolveSpeed;
            }
            else
            {
                //We're done and we don't need this thing any more.
                Destroy(this.gameObject);
            }
        }
    }
}

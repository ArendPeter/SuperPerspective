using UnityEngine;
using System.Collections;

public class Fadeout2D : MonoBehaviour {

    SpriteRenderer playerSprite;
    bool fadeIn = false;
    float timer;
    Color playerCol;

    // Use this for initialization
    void Start () {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        playerCol = playerSprite.color;
        playerSprite.color = new Color(playerCol.r, playerCol.g, playerCol.b, 1);
        FadeOut();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void FixedUpdate()
    {
        if (timer > 0) {
            timer = timer - Time.deltaTime;
        }
        else if (!fadeIn && playerSprite.color.a > 0)
        {
            playerSprite.color = new Color(playerCol.r, playerCol.g, playerCol.b, playerSprite.color.a - 0.02f);
        }
        else if (fadeIn && playerSprite.color.a < 1)
        {
            playerSprite.color = new Color(playerCol.r, playerCol.g, playerCol.b, playerSprite.color.a + 0.02f);
        }
    }

    public void FadeOut()
    {
        timer = 0;
        fadeIn = false;
    }

    public void FadeIn()
    {
        timer = 0.5f;
        fadeIn = true;
    }
}

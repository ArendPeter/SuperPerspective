using UnityEngine;
using System.Collections;

public class Fadeout2D : MonoBehaviour {

    SpriteRenderer playerSprite;
    bool fadeIn = false, done = false;
    public bool totallyDone = false;
    float timer;
    Color playerCol;
    public float fadeTimer = 82;

    public static Fadeout2D instance;

    void Awake()
    {
        instance = this;
    }

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
        if (totallyDone == false && done && playerSprite.color.a >= 1)
        {
            totallyDone = true;
        }
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0)
            {
                FadeIn();
                done = true;
            }
        }
        if (timer > 0)
        {
            timer = timer - Time.deltaTime;
        }
        else if (!fadeIn && playerSprite.color.a > 0)
        {
            playerSprite.color = new Color(playerCol.r, playerCol.g, playerCol.b, playerSprite.color.a - 1f*Time.deltaTime);
        }
        else if (fadeIn && playerSprite.color.a < 1)
        {
            playerSprite.color = new Color(playerCol.r, playerCol.g, playerCol.b, playerSprite.color.a + 1f*Time.deltaTime);
        }
    }

    void FixedUpdate()
    {

    }

    public void FadeOut()
    {
        if (!done){
            timer = 0;
            fadeIn = false;
        }
    }

    public void FadeIn()
    {
        if (!done){
            timer = 0.5f;
            fadeIn = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

    float fadeInTime = 2f, fadeOutTime = 2, delay = 2;
    Vector4 color;
    SpriteRenderer image;

    // Use this for initialization
    void Start () {
        image = GetComponent<SpriteRenderer>();
        color = image.color;
        color[3] = 0;
        image.color = color;
        StartCoroutine(FadeIn());
		AchievementManager.CheckAchievements();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator FadeIn()
    {
        float timer = delay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        timer = fadeInTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            color[3] = 1 - timer / fadeInTime;
            image.color = color;
            yield return null;
        }
        timer = delay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float timer = fadeOutTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            color[3] = timer / fadeInTime;
            image.color = color;
            yield return null;
        }
        /*timer = delay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }*/
        //SceneManager.LoadScene("Menu");
        TransitionManager.instance.SceneTransition(null, "", "Menu");
    }

}

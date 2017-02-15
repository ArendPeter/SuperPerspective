using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigCrystalGet : MonoBehaviour {

    float fadeInTime = 0.5f, fadeOutTime = 2, delay = 2;
    Vector4 color;
    Vector3 scale;
    Image image;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
        scale = image.rectTransform.localScale;
        scale[0] = 0;
        scale[1] = 0;
        image.transform.localScale = scale;
        color = image.color;
        color[3] = 0;
        image.color = color;
        image.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BigCrystalGot()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        image.enabled = true;
        float timer = fadeInTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            color[3] = 1 - timer/fadeInTime;
            scale[0] = 1 - timer / fadeInTime;
            scale[1] = 1 - timer / fadeInTime;
            image.transform.localScale = scale;
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
        image.enabled = false;
    }

}

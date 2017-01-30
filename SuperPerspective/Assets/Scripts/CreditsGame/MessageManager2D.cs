using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageManager2D : MonoBehaviour {

    public List<GameObject> messages;
    public Text levelScore, crystalScore;
    GameObject activeMessage;
    int count = 0;
    float fadeSpeed = 0.05f;
    public float tempTimerMax = 10;
    float tempTimer = 3;

    public static MessageManager2D instance;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (tempTimer > 0)
        {
            tempTimer -= Time.deltaTime;
        }
        else if (count < 9)
        {
            LoadMessage(count, 2);
            count++;
            tempTimer = tempTimerMax;
        }
        else if (count == 9)
        {
            StartCoroutine(FadeOut(activeMessage));
            count++;
            tempTimer = tempTimerMax * 0.5f;
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
	}

    public void LoadMessage(int num, float delay)
    {
        if (activeMessage != null)
        {
            StartCoroutine(FadeOut(activeMessage));
        }
        activeMessage = messages[num];
        StartCoroutine(FadeIn(activeMessage, delay));
    }

    IEnumerator FadeIn(GameObject parent, float delay)
    {
        yield return new WaitForSeconds(delay);
        parent.SetActive(true);
        for (float f = 0f; f <= 1; f += 0.1f)
        {
            AdjustAlphaPlusChildren(parent, f);
            yield return new WaitForSeconds(fadeSpeed);
        }
    }

    IEnumerator FadeOut(GameObject parent)
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            AdjustAlphaPlusChildren(parent, f);
            yield return new WaitForSeconds(fadeSpeed);
        }
        parent.SetActive(false);
    }

    void AdjustAlphaPlusChildren(GameObject parent, float val)
    {
        List<MaskableGraphic> tempChildren = new List<MaskableGraphic>(parent.GetComponentsInChildren<MaskableGraphic>());
        if (tempChildren != null)
        {
            foreach (MaskableGraphic obj in tempChildren)
            {
                obj.color = new Color(obj.color.r, obj.color.b, obj.color.g, val);
            }
        }
    }


}

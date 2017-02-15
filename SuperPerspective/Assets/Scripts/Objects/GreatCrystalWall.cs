using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreatCrystalWall : MonoBehaviour {

    public Activatable[] switches;
    bool grass = false, ice = false, desert = false, activated = false;
    public GameObject c1, c2, c3;
    public TextMesh bigCrystalText;
    int bigCrystalCount = 0;
    int littleCrystalCount = 0, maxLittleCrystalCount = 0;
    public CrystalCountText tutCount, grassCount, iceCount, desertCount;
    public bool gotAllSmallCrystals = false;
    public convoNode creditsConvo;
    public convoNode failureConvo;


    // Use this for initialization
    void Start() {
//        Debug.Log("Grass Scene: " + PlayerPrefs.GetInt("GrassScene"));
        if (PlayerPrefs.GetInt("GrassScene") == 1){
            grass = true;
            c1.SetActive(true);
            bigCrystalCount++;
        }
        if (PlayerPrefs.GetInt("DesertScene") == 1)
        {
            desert = true;
            c2.SetActive(true);
            bigCrystalCount++;
        }
        if (PlayerPrefs.GetInt("IceScene") == 1)
        {
            ice = true;
            c3.SetActive(true);
            bigCrystalCount++;
        }
        bigCrystalText.text = "Big Crystals: " + bigCrystalCount + "/3";
        littleCrystalCount = tutCount.currentCount + grassCount.currentCount + iceCount.currentCount + desertCount.currentCount;
        maxLittleCrystalCount = tutCount.maxCount + grassCount.maxCount + iceCount.maxCount + desertCount.maxCount;
        if (littleCrystalCount >= maxLittleCrystalCount)
        {
            gotAllSmallCrystals = true;
        }
        //Debug.Log("Current crystals: " + littleCrystalCount);
        //Debug.Log("Max crystals: " + maxLittleCrystalCount);
    }

    public void Activate()
    {
        if (!activated)
        {
            if (bigCrystalCount >= 3)
            {
                //Crystal Fairy closing dialogue
                textBoxScript.instance.startConvo(creditsConvo);
                //TODO Larry, call this class's OpenGate function after the dialogue
                //OpenGate();
            }
            else
            {
                //Crystal Fairy tells you to gather big crystals
                textBoxScript.instance.startConvo(failureConvo);
            }
        }
    }

    public void OpenGate()
    {
        for (int i = 0; i < switches.Length; i++)
        {
            switches[i].activated = true;
            activated = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

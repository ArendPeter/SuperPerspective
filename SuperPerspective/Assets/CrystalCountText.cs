using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMesh))]

public class CrystalCountText : MonoBehaviour {

	public string sceneName;
	public int maxCount;
    public int currentCount;

    void Start()
    {
        UpdateValues();
    }

    public void UpdateValues()
    {
        TextMesh txtmesh = GetComponent<TextMesh>();
        Text pauseText = GetComponent<Text>();
        bool hasFinalCrystal = PlayerPrefs.GetInt(sceneName + "BigCrystal") == 1;
        currentCount = 0;

        if (PlayerPrefs.HasKey(this.sceneName + "CollectableList"))
        {
            string[] colList = PlayerPrefs.GetString(this.sceneName + "CollectableList").Split(","[0]);
            currentCount = colList.Length;
        }

        //print(pauseText);

        if ((hasFinalCrystal || sceneName == "TutorialScene") && currentCount == this.maxCount)
        {
            string completeMessage = "All Crystals Found!";
            if (txtmesh != null)
            {
                txtmesh.text = completeMessage;
            }
            if (pauseText != null)
            {
                pauseText.text = completeMessage;
            }
        }
        else
        {
            string completeMessage = "Crystals " + currentCount + "/" + this.maxCount;
            if (hasFinalCrystal)
            {
                completeMessage += " + Final";
            }

            if (txtmesh != null)
            {
                txtmesh.text = completeMessage;
            }
            if (pauseText != null)
            {
                pauseText.text = completeMessage;
            }

        }
    }
}

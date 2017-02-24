using UnityEngine;
using System.Collections;

public class ControllerUI_Update : MonoBehaviour {

    public GameObject[] xbox, pc;
    bool pcEnabled = true;

	// Use this for initialization
	void Start () {
        EnablePCUI();
    }
	
	// Update is called once per frame
	void Update () {
        if (pcEnabled && Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0] != "") {
            EnableXboxUI();
            //Debug.Log("Jostick: " + Input.GetJoystickNames()[0]);
        }
        else if (!pcEnabled && Input.GetJoystickNames().Length == 0)
        {
            EnablePCUI();
            //Debug.Log("Jostick: " + Input.GetJoystickNames().Length);
        }
    }

    void EnableXboxUI()
    {
        pcEnabled = false;
        for (int i = 0; i < xbox.Length; i++)
        {
            if(xbox[i] != null)
                xbox[i].SetActive(true);
        }
        for (int i = 0; i < pc.Length; i++)
        {
            if (pc[i] != null)
                pc[i].SetActive(false);
        }
    }

    void EnablePCUI()
    {
        pcEnabled = true;
        for (int i = 0; i < xbox.Length; i++)
        {
            xbox[i].SetActive(false);
        }
        for (int i = 0; i < pc.Length; i++)
        {
            pc[i].SetActive(true);
        }
    }
}

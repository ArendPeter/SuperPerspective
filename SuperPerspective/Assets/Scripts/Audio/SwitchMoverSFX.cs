using UnityEngine;
using System.Collections;

public class SwitchMoverSFX : MonoBehaviour {

    public AudioSource moving, stop;

    public void StartSFX()
    {
        //Debug.Log("TryStart");
        if (!moving.isPlaying)
        {
            moving.Play();
            //Debug.Log("Start");
        }
    }

    public void StopSFX()
    {
        //Debug.Log("TryStop");
        if (moving.isPlaying)
        {
            moving.Stop();
            stop.Stop();
            stop.Play();
            //Debug.Log("Stop");
        }
    }
}

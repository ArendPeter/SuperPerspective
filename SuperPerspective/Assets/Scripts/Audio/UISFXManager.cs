using UnityEngine;
using System.Collections;

public class UISFXManager : MonoBehaviour {

    public AudioSource music, menuMoveSFX;
    bool fadeMusic = false;
    float fadeRate = 1;
    float musicVol;

	// Use this for initialization
	void Start () {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicVol = music.volume;
	}
	
	// Update is called once per frame
	void Update () {
        if (fadeMusic)
        {
            music.volume -= Time.deltaTime*fadeRate;
            if (music.volume <= 0)
            {
                fadeMusic = false;
            }
        }
	}

    public void PlayMenuMove()
    {
        Debug.Log("Play SFX");
        menuMoveSFX.Play();
    }

    public void FadeOutMusic()
    {
        fadeMusic = true;
    }

    public void PlayMusic()
    {
        music.volume = musicVol;
        music.Play();
    }
}

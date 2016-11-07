using UnityEngine;
using System.Collections;

public class UISFXManager : MonoBehaviour {

    public AudioSource music, menuMoveSFX, fairyTheme;
    bool fadeMusic = false, playFairyTheme = false, fadeFairyTheme = false;
    float fadeRate = 0.3f;
    float musicVol, fairyThemeVol;

	// Use this for initialization
	void Start () {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicVol = music.volume;
        fairyThemeVol = fairyTheme.volume;
    }
	
	// Update is called once per frame
	void Update () {
        if (fadeMusic)
        {
            music.volume -= Time.deltaTime*fadeRate;
            if (music.volume <= 0)
            {
                fadeMusic = false;
                if (playFairyTheme)
                {
                    PlayFairyTheme();
                    playFairyTheme = false;
                }
            }
        }
        else if (fadeFairyTheme)
        {
            fairyTheme.volume -= Time.deltaTime * fadeRate;
            if (fairyTheme.volume <= 0)
            {
                fadeFairyTheme = false;
                PlayMusic();
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

    public void PlayCrystalFairyTheme()
    {
        fadeMusic = true;
        playFairyTheme = true;
    }
    public void StopCrystalFairyTheme()
    {
        fadeFairyTheme = true;
    }

    public void PlayMusic()
    {
        music.volume = musicVol;
        music.Play();
    }

    public void PlayFairyTheme()
    {
        fairyTheme.volume = musicVol;
        fairyTheme.Play();
    }
}

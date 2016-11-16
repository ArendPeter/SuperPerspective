using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class UISFXManager : MonoBehaviour {

    public AudioSource music, menuMoveSFX, fairyTheme;
    bool fadeMusic = false, playFairyTheme = false, fadeFairyTheme = false, fadeEverything = false;
    float fadeRateInit = 0.3f, fadeRate, mixVolume = 18;
    float musicVol, fairyThemeVol;

    public AudioMixer masterMixer;

	// Use this for initialization
	void Start () {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicVol = music.volume;
        fairyThemeVol = fairyTheme.volume;
        SetMasterVol(mixVolume);
        fadeRate = fadeRateInit;
    }
	
	// Update is called once per frame
	void Update () {
        if (fadeEverything) {
            float tempVol = GetMasterVol();
            SetMasterVol(tempVol - Time.deltaTime * fadeRate);
            if (tempVol <= 0)
            {
                fadeEverything = false;
            }
        }
        else if (fadeMusic)
        {
            music.volume -= Time.deltaTime * fadeRate;
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
        fadeRate = fadeRateInit;
    }

    public void FadeOutEverything()
    {
        fadeEverything = true;
        fadeRate = fadeRateInit*60f;
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

    void SetMasterVol(float masterVol)
    {
        masterMixer.SetFloat("MasterVol", masterVol);
    }

    float GetMasterVol()
    {
        float temp;
        masterMixer.GetFloat("MasterVol", out temp);
        return temp;
    }
}

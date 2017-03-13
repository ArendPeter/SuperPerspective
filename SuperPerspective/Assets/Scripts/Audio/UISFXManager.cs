using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UISFXManager : MonoBehaviour {

    public AudioSource music, menuMoveSFX, fairyTheme;
    bool fadeMusic = false, fadeInMusic = false, playFairyTheme = false, fadeFairyTheme = false, fadeEverything = false, fadeInFairy = false;
    float fadeRateInit = 0.3f, fadeRate, mixVolume = 18;
    float musicVol, fairyThemeVol;

    public AudioMixer masterMixer;

	// Use this for initialization
	void Start () {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        musicVol = music.volume;
		if (fairyTheme != null) {
        	fairyThemeVol = fairyTheme.volume;
		}
        SetMasterVol(mixVolume);
        fadeRate = fadeRateInit;
    }
	
	// Update is called once per frame
	void Update () {
        if (fadeEverything)
        {
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
                music.Pause();
                fadeMusic = false;
                if (playFairyTheme)
                {
                    PlayFairyTheme();
                    playFairyTheme = false;
                }
            }
        }
        else if (fadeInFairy)
        {
            if (fairyTheme.volume < fairyThemeVol)
            {
                fairyTheme.volume = Mathf.Min(fairyThemeVol, fairyTheme.volume += Time.deltaTime * fadeRate);
            }
            else
            {
                fadeInFairy = false;
            }
        }
        else if (fadeFairyTheme)
        {
            fairyTheme.volume -= Time.deltaTime * fadeRate;
            if (fairyTheme.volume <= 0)
            {
                fadeFairyTheme = false;
                fairyTheme.Stop();
                FadeInMusic();
            }
        }
        else if (fadeInMusic)
        {
            if (music.volume < musicVol)
            {
                music.volume = Mathf.Min(musicVol, music.volume += Time.deltaTime * fadeRate);
            }
            else
            {
                fadeInMusic = false;
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

        //If statement is hotfix for menu bug where theme doesn't play
        if (SceneManager.GetActiveScene().name != "Hub")
        {
            //Debug.Log("Play song");
            if (!fairyTheme.isPlaying)
            {
                FadeOutMusic();
            }
            else
            {
                fadeInFairy = true;
            }
            playFairyTheme = true;
            fadeFairyTheme = false;
        }
    }
    public void StopCrystalFairyTheme()
    {

        //If statement is hotfix for menu bug where theme doesn't play
        if (SceneManager.GetActiveScene().name != "Hub")
        {
            playFairyTheme = false;
            fadeInFairy = false;
            if (fairyTheme.isPlaying)
            {
                fadeFairyTheme = true;
                fadeRate = fadeRateInit;
            }
            else
            {
                FadeInMusic();
            }
        }
    }

    public void PlayMusic()
    {
        music.volume = musicVol;
        music.Play();
    }

    public void FadeInMusic()
    {
        fadeInFairy = false;
        fadeFairyTheme = false;
        fadeMusic = false;
        playFairyTheme = false;
        music.volume = 0;
        music.UnPause();
        fadeInMusic = true;
    }

    public void PlayFairyTheme()
    {
        fairyTheme.volume = fairyThemeVol;
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

    //For big crystal fanfare
    public void PauseMusicSetVolumeToZero()
    {
        music.Pause();
        music.volume = 0;
    }
}

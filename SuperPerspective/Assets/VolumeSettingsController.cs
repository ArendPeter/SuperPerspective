using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettingsController : MonoBehaviour {

	public float musicVol = 1f;
	public float sfxVol = 1f;
	const float BASE_MUSIC_VOL = -16.04f;
	const float BASE_SFX_VOL = -5.72f;
	const float MIN_VOL = -80.00f;

	public Slider musicSlider;
	public Slider sfxSlider;
	public AudioMixer mixer;
    public AudioSource audioS;

    float sliderNoiseTimer = 0;

	void Start() {
		if (!PlayerPrefs.HasKey("MusicVol")) {
			PlayerPrefs.SetFloat("MusicVol", 1f);
		} else {
			musicVol = PlayerPrefs.GetFloat("MusicVol");
			musicSlider.value = musicVol;
			mixer.SetFloat("musicVol", Mathf.Lerp(MIN_VOL, BASE_MUSIC_VOL, Mathf.Log10(musicVol)));
		}
		if (!PlayerPrefs.HasKey("SfxVol")) {
			PlayerPrefs.SetFloat("SfxVol", 1f);
		} else {
			sfxVol = PlayerPrefs.GetFloat("SfxVol");
			musicSlider.value = sfxVol;
			mixer.SetFloat("sfxVol", Mathf.Lerp(MIN_VOL, BASE_SFX_VOL, Mathf.Log10(sfxVol)));
		}
	}

    public void Update()
    {
        if (sliderNoiseTimer > 0)
        {
            sliderNoiseTimer -= Time.deltaTime;
        }
    }

	public float getMusicVol() {
		return musicVol;
	}

	public float getSfxVol() {
		return sfxVol;
	}

	public void setMusicVol() {
		musicVol = musicSlider.value;
		PlayerPrefs.SetFloat("MusicVol", musicVol);
		mixer.SetFloat("musicVol", Mathf.Lerp(MIN_VOL, BASE_MUSIC_VOL, Mathf.Log10(musicVol)));
	}

	public void setSfxVol() {
        //Play noise as volume slider is changed
        if (sliderNoiseTimer <= 0)
        {
            audioS.Play();
            sliderNoiseTimer = 0.1f;
        }

		sfxVol = sfxSlider.value;
		PlayerPrefs.SetFloat("SfxVol", sfxVol);
		mixer.SetFloat("sfxVol", Mathf.Lerp(MIN_VOL, BASE_SFX_VOL, Mathf.Log10(sfxVol)));
	}
}

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
		sfxVol = sfxSlider.value;
		PlayerPrefs.SetFloat("SfxVol", sfxVol);
		mixer.SetFloat("sfxVol", Mathf.Lerp(MIN_VOL, BASE_SFX_VOL, Mathf.Log10(sfxVol)));
	}
}

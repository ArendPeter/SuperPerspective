using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsController : MonoBehaviour {

	public static VolumeSettingsController instance = null;
	float musicVol = 1f;
	float sfxVol = 1f;

	public Slider musicSlider;
	public Slider sfxSlider;

	void Start() {
		if (instance == null) {
			instance = this;
		}
		if (!PlayerPrefs.HasKey("MusicVol")) {
			PlayerPrefs.SetFloat("MusicVol", 1f);
		} else {
			musicVol = PlayerPrefs.GetFloat("MusicVol");
			musicSlider.value = musicVol;
		}
		if (!PlayerPrefs.HasKey("SfxVol")) {
			PlayerPrefs.SetFloat("SfxVol", 1f);
		} else {
			sfxVol = PlayerPrefs.GetFloat("SfxVol");
			sfxSlider.value = sfxVol;
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
	}

	public void setSfxVol() {
		sfxVol = sfxSlider.value;
		PlayerPrefs.SetFloat("SfxVol", sfxVol);
		foreach (AudioSource audio in FindObjectsOfType<AudioSource>()) {
			if (audio.outputAudioMixerGroup.name == "SFX") {
				audio.volume = sfxVol;
			}
		}
	}
}

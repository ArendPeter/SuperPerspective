using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeControl : MonoBehaviour {

	AudioSource audio;
	float baseVol;

	void Start() {
		audio = this.GetComponent<AudioSource>();
		baseVol = audio.volume;
	}

	void FixedUpdate () {
		float vol = VolumeSettingsController.instance.getMusicVol();
		audio.volume = baseVol * vol;
	}
}

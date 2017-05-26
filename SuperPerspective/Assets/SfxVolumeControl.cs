using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxVolumeControl : MonoBehaviour {

	private Hashtable baseVolumes = new Hashtable();

	void Start() {
		foreach (AudioSource audio in FindObjectsOfType<AudioSource>()) {
			if (audio.outputAudioMixerGroup.name == "SFX") {
				baseVolumes.Add(audio, audio.volume);
			}
		}
	}

	// Update is called once per frame
	void LateUpdate () {
		foreach (AudioSource audio in baseVolumes.Keys) {
			float baseVol = (float)baseVolumes[audio];
			if (audio.outputAudioMixerGroup.name == "SFX") {
				audio.volume = Mathf.Min(Mathf.Min(VolumeSettingsController.instance.getSfxVol() * baseVol, audio.volume), baseVol);
			}
		}
	}
}

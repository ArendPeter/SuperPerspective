using UnityEngine;
using System.Collections;

public class PushSwitch : MonoBehaviour {

	#pragma warning disable 114, 414

	float alpha, fade;
	Renderer rune;
	Color color;
	ParticleSystem particles;

	Vector3 baseScale;

	void Start() {
		rune = GetComponentInChildren<Renderer>();
		color = Color.white;
		particles = GetComponentInChildren<ParticleSystem>();
	}

	void Update(){
		if (Physics.Raycast(transform.position, Vector3.up, 0.5f)) {
			fade = -1/50f;
		} else {
			fade = 1/50f;
		}
		color.a = alpha;
		rune.material.SetColor("_Color", color);
		if (alpha > 0.5 && !particles.isPlaying) {
			particles.Play();
		} else if (alpha <= 0.5 && particles.isPlaying) {
			particles.Stop();
		}
	}

	void FixedUpdate() {
		rune.transform.RotateAround (transform.position, Vector3.up, 1);
		alpha = Mathf.Clamp(alpha + fade, 0, 1);
	}

}

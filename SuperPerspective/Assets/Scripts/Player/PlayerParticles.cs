using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerParticles : MonoBehaviour {

	private PlayerController player;

	public ParticleSystem[] particlesList;
	private Dictionary<string, ParticleSystem> ParticleDictionary;

	public static PlayerParticles instance;

	void Start () {
		organizeParticles();
		initPlayerReference();
		initEmitters();
	}

	private void organizeParticles() {
		ParticleDictionary = new Dictionary<string, ParticleSystem>();
		foreach(ParticleSystem particle in particlesList) {
			ParticleDictionary.Add(particle.name, particle);
		}
	}
	
	private void initPlayerReference(){ player = PlayerController.instance; }
	
	private void initEmitters(){ 
		foreach(ParticleSystem particle in particlesList) {
			particle.enableEmission = false;
		}
	}
	
	
	void FixedUpdate () {
		if(!player.isDisabled())
			updateParticleEmission();
	}
	
	private void updateParticleEmission(){
		ParticleDictionary["DustEmitter"].enableEmission = (player.isRunning() || player.isWalking()) && player.isGrounded();
	}

	public void startSparkle(){
		ParticleDictionary["Sparkle Particle"].enableEmission = true;
	}
}

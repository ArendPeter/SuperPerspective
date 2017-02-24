using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ParticleSystem))]

public class AmbientParticleHandler : MonoBehaviour {

	public bool disableParticles = false;
	public bool instantiateSystems = false;//check to create new instances, otherwise it will use the children
	public ParticleSystem PrefabParticles2D;//particles to clone
	public ParticleSystem PrefabParticles3D;

	private ParticleSystem P2D;//reference
	private ParticleSystem P3D;

	private GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		
		if(instantiateSystems){
			P2D = (ParticleSystem) Instantiate(PrefabParticles2D, Vector3.zero, Quaternion.identity);
			P3D = (ParticleSystem) Instantiate(PrefabParticles3D, Vector3.zero, Quaternion.identity);

			Destroy(PrefabParticles2D.gameObject);
			Destroy(PrefabParticles3D.gameObject);

			if(player != null){
				P2D.transform.SetParent(player.transform, false);
				P3D.transform.SetParent(player.transform, false);
				
				P2D.transform.position = player.transform.position;
				P3D.transform.position = player.transform.position;
			}
		} else {
			P2D = PrefabParticles2D;
			P3D = PrefabParticles3D;

			if(player != null){
				this.transform.SetParent(player.transform);
				this.transform.position = Vector3.zero;
				//print("zero");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(P2D != null && P3D != null){
			if(GameStateManager.is3D()){
				P2D.enableEmission = false;
				P3D.enableEmission = !disableParticles;
			}else{
				P2D.enableEmission = !disableParticles;
				P3D.enableEmission = false;
			}
		}
	}
}

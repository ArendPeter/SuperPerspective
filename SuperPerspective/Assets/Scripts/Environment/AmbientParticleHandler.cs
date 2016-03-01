using UnityEngine;
using System.Collections;

public class AmbientParticleHandler : MonoBehaviour {

	public bool disableParticles = false;
	public bool instantiateSystems;//check to use prefabs, uncheck if you want to pass in obj
	public ParticleSystem PrefabParticles2D;//particles to clone
	public ParticleSystem PrefabParticles3D;

	private ParticleSystem P2D;//reference
	private ParticleSystem P3D;
	// Use this for initialization
	void Start () {
		if(instantiateSystems){
			P2D = (ParticleSystem) Instantiate(PrefabParticles2D, Vector3.zero, Quaternion.identity);
			//P2D.transform.SetParent(this.transform, false);
			//P2D.transform.position = new Vector3(0, 0, 0);

			P3D = (ParticleSystem) Instantiate(PrefabParticles3D, Vector3.zero, Quaternion.identity);
			//P3D.transform.SetParent(this.transform, false);
			//P3D.transform.position = new Vector3(0, 0, 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(P2D != null && P3D != null){
			P2D.transform.position = this.transform.position;
			P3D.transform.position = this.transform.position;

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

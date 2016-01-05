using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	//public Door dest;
	public string myName;
	public string destName;
	Door destDoor;
	public Color particleColor;

	public bool isSceneLoad;
	public int crystalRequirement;

    public GameObject warpSound;

    ListenerHandler l;

	public void Awake(){
		//update particle color
		ParticleSystem p = this.transform.FindChild("Particles").GetComponent<ParticleSystem>();
        l = GameObject.Find("AudioListener").GetComponent<ListenerHandler>();
        p.startColor = particleColor;
		p.Simulate(2f);
		p.Play();
        range = 4;
	}

	public override float GetDistance() {
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			return Vector3.Distance(transform.position, player.transform.position);
		else
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
			                        new Vector2(transform.position.x, transform.position.y));
	}

	public override void Triggered(){

        //Creates game object that plays warp sound then self destructs -Nick
        Instantiate(warpSound);

		if(isSceneLoad && destName != null)
			Application.LoadLevel(destName);

		else if(destDoor!=null && MainCollectable.GetMainCollectableHeld() >= crystalRequirement)
			player.GetComponent<PlayerController>().Teleport(
				destDoor.GetComponent<Collider>().bounds.center + new Vector3(0,0,-2));
		else
			Debug.Log("Door not linked");

        l.ResetZ();
    }

	public string getName(){
		return myName;
	}
	
	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}
	
}


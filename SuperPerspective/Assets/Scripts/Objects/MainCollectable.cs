using UnityEngine;
using System.Collections;

public class MainCollectable : MonoBehaviour {

	#pragma warning disable 472, 1692

	static int collectableHeld = 0;
	bool active = true;
	bool consumed = false;
	float range = 2f;
	Vector3 posOnPlayer = new Vector3(0,2f,0);

	void FixedUpdate() {
		if (active)
			GetComponentInChildren<Renderer>().transform.Rotate(Vector3.up, Mathf.PI / 4, Space.World);

		if(active){
			checkForPlayerInGrabRange();
		}else if(!consumed){
			spiralToPlayer();
		}
	}

	public ParticleSystem effectOnCollect;

	public static bool UseKey(int amtRequired) {
		amtRequired = 1;
		if (collectableHeld >= amtRequired) {
			collectableHeld--;
			return true;
		}
		return false;
	}

	public static void Collect() {
		collectableHeld++;
    }

	public static void GiveMainCollectable(int amt) {
		collectableHeld = collectableHeld + amt;
	}

	public static void ClearMainCollectable() {
		collectableHeld = 0;
	}

	public static int GetMainCollectableHeld(){
		return collectableHeld;
	}

	private void checkForPlayerInGrabRange(){
		Bounds mybound = GetComponent<BoxCollider>().bounds;
		Bounds pbound = PlayerController.instance.GetComponent<BoxCollider>().bounds;
		bool intersect = mybound.Intersects(pbound);
		if(active && intersect){
			active = false;
            playSFX();
        }
	}

	private void spiralToPlayer(){
		Vector3 targetPos = PlayerController.instance.transform.position + posOnPlayer;
		transform.position = SpiralPath.SpiralPositionTo(transform.position, targetPos);
		float dist = (transform.position - targetPos).magnitude;
		if(dist < .01){
			if(effectOnCollect != null){
				Instantiate(effectOnCollect, this.transform.position, Quaternion.identity);// PlayerController.instance.transform.position
			}
			gameObject.SetActive(false);
			consumed = true;
		}
	}

    //Plays SFX upon pickup -Nick
    private void playSFX()
    {
        GetComponent<AudioSource>().Play();
    }
}

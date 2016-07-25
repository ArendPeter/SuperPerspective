using UnityEngine;
using System.Collections;

public class MainCollectable : MonoBehaviour {

	#pragma warning disable 472, 1692

	static int collectableHeld = 0;
	bool active = true;
	bool consumed = false;
	float range = 2f;
	Vector3 posOnPlayer = new Vector3(0,2f,0);
    public GameObject sound;
	static Color collectedColor = new Color(0.3f, 0.3f, 0.9f, 0.3f);

	private string uid;

	void Start() {
		uid = transform.position.x + "" + transform.position.y + "" + transform.position.z;
		if (PlayerPrefs.HasKey(uid)) {
			foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
				rend.material.SetColor("_EmissionColor", collectedColor);
			}
			ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
			ps.startColor = collectedColor;
		}
	}

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
			// Flag the collectable as collected
			PlayerPrefs.SetInt(uid, 1);
			// Update saved value for collectableHeld
			if (PlayerPrefs.HasKey("CollectableHeld")) {
				PlayerPrefs.SetInt("CollectableHeld", 1);
			} else {
				PlayerPrefs.SetInt("CollectableHeld", PlayerPrefs.GetInt("CollectableHeld") + 1);
			}
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
        Instantiate(sound);
    }
}

using UnityEngine;
using System.Collections;

public class MainCollectable : MonoBehaviour {

	#pragma warning disable 472, 1692

	public bool isFinalCollectable = false;
	static int collectableHeld = 0;
	bool active = true;
	bool consumed = false;
	float range = 2f;
	Vector3 posOnPlayer = new Vector3(0,2f,0);
    public GameObject sound;
	static Color collectedColor = new Color(0.3f, 0.3f, 0.9f, 0.3f);
	string sceneName;
	public Activatable[] triggers;
	public bool isEndCrystal;

	private string uid;

	void Start() {
		sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		uid = transform.position.x + "" + transform.position.y + "" + transform.position.z;
		if (PlayerPrefs.HasKey(uid)) {
			foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
				rend.material.SetColor("_EmissionColor", collectedColor);
			}
			ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
			ps.startColor = collectedColor;
            if (isEndCrystal)
            {
                GameObject.Destroy(this.gameObject);
            }
		}

		this.isPickedUp();
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
			GenericDissolver dissolver = GetComponentInChildren<GenericDissolver>();
			if(dissolver){
					dissolver.shouldDissolveObject = true;
					dissolver.dissolveAmount = -1.2f;
			}
			active = false;
            playSFX();

            if (isEndCrystal)
            {
                playUI();
            }

            bool wasPickedUp = this.isPickedUp();
			// Flag the collectable as collected
			PlayerPrefs.SetInt(uid, 1);
			if (isEndCrystal) {
				// Mark the level as beaten
				PlayerPrefs.SetInt(sceneName, 1);
			} else if (PlayerPrefs.HasKey("CollectableHeld")) { // Update number of collectables held
				PlayerPrefs.SetInt("CollectableHeld", 1);
				if(!wasPickedUp) {
					this.addToCollectableList();
				}
			} else {
				PlayerPrefs.SetInt("CollectableHeld", PlayerPrefs.GetInt("CollectableHeld") + 1);
				if(!wasPickedUp) {
					this.addToCollectableList();
				}
			}
        }
	}

	private void spiralToPlayer(){
		Vector3 targetPos = PlayerController.instance.transform.position + posOnPlayer*((isFinalCollectable)?1.5f:1);
		transform.position = SpiralPath.SpiralPositionTo(transform.position, targetPos, isFinalCollectable );
		float dist = (transform.position - targetPos).magnitude;
		if(dist < .01){
			Saved_UI.instance.ShowSaved();

			if(effectOnCollect != null){
				Instantiate(effectOnCollect, this.transform.position, Quaternion.identity);// PlayerController.instance.transform.position
			}
			gameObject.SetActive(false);
			consumed = true;
			foreach(Activatable o in triggers)
				o.setActivated(true);
		}
	}

	private bool isPickedUp() {
		if(PlayerPrefs.HasKey(sceneName+"CollectableList")) {
			string[] colList = PlayerPrefs.GetString(sceneName+"CollectableList").Split(","[0]);
			if(colList[0] == ""){
				return false;
			}
			if(colList.Length > 0){
				return System.Array.IndexOf(colList, this.uid) > -1;
			}
		}
		return false;
	}
	private void addToCollectableList() {

		string colString = PlayerPrefs.GetString(sceneName+"CollectableList");
		if(colString == ""){
			PlayerPrefs.SetString(sceneName+"CollectableList", uid);
			Saved_UI.instance.ShowSaved();
		} else {
			PlayerPrefs.SetString(sceneName+"CollectableList", string.Concat(colString, "," + uid));
			Saved_UI.instance.ShowSaved();
		}
	}

    //Plays SFX upon pickup -Nick
    private void playSFX()
    {
        Instantiate(sound);
    }

    //Play UI upon big crystal pickup
    private void playUI()
    {
        BigCrystalGet temp = FindObjectOfType<BigCrystalGet>();
        if (temp != null)
        {
            temp.BigCrystalGot();
        }
    }
}

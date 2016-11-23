using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour {

	public static TransitionManager instance;

	const float fadeSpeed = 1 / 2f;
	public Texture transitionTexture;

	static bool move = false;
	static string toDoor;
	static float alpha = 0f;
	static int transition = 0;
	static string gotoScene = "", gotoDoor = "";
	PlayerController player;

    UISFXManager uiSFXManager;

	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(instance);
	}

	void Start() {
		player = PlayerController.instance;
        uiSFXManager = FindObjectOfType<UISFXManager>();
    }

	void OnGUI() {
		if (transition != 0) {
			Color col;
			alpha += transition * fadeSpeed * Time.deltaTime;

			col = GUI.color;
			col.a = alpha;
			GUI.color = col;
			GUI.depth = -1000;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), transitionTexture);

			if (alpha > 1 && transition > 0) {
				alpha = 2;
				transition = -transition;
				DoTransition();
			} else if (alpha < 0) {
				alpha = 0f;
				transition = 0;
				player.setCutsceneMode(false);
			}
		}
	}

	void FixedUpdate() {
		if (move) {
			Vector3 movePos = DoorManager.instance.getDoor(toDoor).gameObject.transform.position + Vector3.up * 2;
			player.transform.position = movePos;
			if (transition != 0)
				player.setCutsceneMode(true);
			move = false;
		}
	}

	void DoTransition() {
        ResetController.LoadScene(gotoScene);
		player = PlayerController.instance;
		if (gotoDoor != "") {
			MovePlayerToDoor (player, gotoDoor);
		}
	}

	public void MovePlayerToDoor(PlayerController p, string door) {
		toDoor = door;
		move = true;
	}

	public void SceneTransition(PlayerController p, string door, string scene) {
        if (uiSFXManager != null)
        {
            uiSFXManager.FadeOutEverything();
        }
		if (p != null) {
			p.setCutsceneMode (true);
		}
		transition = 1;
		gotoScene = scene;
		gotoDoor = door;
	}

	public bool isActive() {
		return instance != null;
	}
}

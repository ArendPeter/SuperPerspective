using UnityEngine;
using System.Collections;

public class ResetController : MonoBehaviour {

	static bool camReset = false;

	void Update () {
		if (Input.GetKey(KeyCode.R))
			Reset ();
	}

	void FixedUpdate() {
		if (camReset) {
			GameStateManager.instance.Reset();
			PlayerController.instance.Reset();
			camReset = false;
		}
	}

	public static void LoadScene(string scene) {
		Key.ClearKeys();
		camReset = true;
		IslandControl.Destroy(IslandControl.instance);
		InputManager.Destroy(InputManager.instance);
		CameraController.Destroy(CameraController.instance);
		CheckpointManager.Destroy(CheckpointManager.instance);
		DoorManager.Destroy(DoorManager.instance);
		EdgeManager.Destroy(EdgeManager.instance);
		PauseMenu.Destroy(PauseMenu.instance);
		TransitionManager.Destroy(TransitionManager.instance);
		Application.LoadLevel(scene);
	}

	public static void Reset() {
		Key.ClearKeys();
		camReset = true;
		IslandControl.Destroy(IslandControl.instance);
		InputManager.Destroy(InputManager.instance);
		CameraController.Destroy(CameraController.instance);
		CheckpointManager.Destroy(CheckpointManager.instance);
		DoorManager.Destroy(DoorManager.instance);
		EdgeManager.Destroy(EdgeManager.instance);
		PauseMenu.Destroy(PauseMenu.instance);
		TransitionManager.Destroy(TransitionManager.instance);
		Application.LoadLevel(Application.loadedLevel);
	}
}

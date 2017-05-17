using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour {
	static string[] SCENE_NAMES = { "TutorialScene", "GrassScene", "DesertScene", "IceScene" };
	static int[] CRYSTAL_COUNTS = { 5, 9, 9, 8 };

	public static void CheckAchievements() {
		bool[] level_clear = { false, false, false };
		bool[] all_crystals = { false, false, false, false };

		for (int i = 1; i < 4; i++) {
			level_clear[i - 1] = (PlayerPrefs.GetInt(SCENE_NAMES[i]) == 1);
		}
		for (int i = 0; i < 4; i++) {
			string sceneName = SCENE_NAMES[i];
			int maxCount = CRYSTAL_COUNTS[i], currentCount;

			if (PlayerPrefs.HasKey(sceneName+"CollectableList")) {
				string[] colList = PlayerPrefs.GetString(sceneName+"CollectableList").Split(","[0]);
				currentCount = colList.Length;
				all_crystals[i] = (currentCount >= maxCount);
			}
		}

		if (level_clear[0] && !PlayerPrefs.HasKey("Achievement_Complete_" + SCENE_NAMES[1])) {
			GrassCompleted();
		}
		if (level_clear[1] && !PlayerPrefs.HasKey("Achievement_Complete_" + SCENE_NAMES[2])) {
			DesertCompleted();
		}
		if (level_clear[2] && !PlayerPrefs.HasKey("Achievement_Complete_" + SCENE_NAMES[3])) {
			IceCompleted();
		}
		if (PlayerPrefs.GetInt("GameComplete") == 1 && !PlayerPrefs.HasKey("Achievement_Complete_Game")) {
			GameCompleted();
		}
		if (all_crystals[1] && !PlayerPrefs.HasKey("Achievement_Crystals_" + SCENE_NAMES[1])) {
			GrassAllCrystals();
		}
		if (all_crystals[2] && !PlayerPrefs.HasKey("Achievement_Crystals_" + SCENE_NAMES[2])) {
			DesertAllCrystals();
		}
		if (all_crystals[3] && !PlayerPrefs.HasKey("Achievement_Crystals_" + SCENE_NAMES[3])) {
			IceAllCrystals();
		}
		if (all_crystals[0] && all_crystals[1] && all_crystals[2] && all_crystals[3] && !PlayerPrefs.HasKey("Achievement_Crystals_Game")) {
			GameAllCrystals();
		}
	}

	static void GrassCompleted() {
		SteamManager.Instance.AchieveBeatGrass();
		PlayerPrefs.SetInt("Achievement_Complete_" + SCENE_NAMES[1], 1);
		PlayerPrefs.Save();
		Debug.Log("GRASS COMPLETE");
	}

	static void GrassAllCrystals() {
		SteamManager.Instance.AchieveAllCrystalsInGrass();
		PlayerPrefs.SetInt("Achievement_Crystals_" + SCENE_NAMES[1], 1);
		PlayerPrefs.Save();
		Debug.Log("GOT ALL CRYSTALS IN GRASS");
	}

	static void DesertCompleted() {
		SteamManager.Instance.AchieveBeatDesert();
		PlayerPrefs.SetInt("Achievement_Complete_" + SCENE_NAMES[2], 1);
		PlayerPrefs.Save();
		Debug.Log("DESERT COMPLETE");
	}

	static void DesertAllCrystals() {
		SteamManager.Instance.AchieveAllCrystalsInDesert();
		PlayerPrefs.SetInt("Achievement_Crystals_" + SCENE_NAMES[2], 1);
		PlayerPrefs.Save();
		Debug.Log("GOT ALL CRYSTALS IN DESERT");
	}

	static void IceCompleted() {
		SteamManager.Instance.AchieveBeatIce();
		PlayerPrefs.SetInt("Achievement_Complete_" + SCENE_NAMES[3], 1);
		PlayerPrefs.Save();
		Debug.Log("ICE COMPLETE");
	}

	static void IceAllCrystals() {
		SteamManager.Instance.AchieveAllCrystalsInIce();
		PlayerPrefs.SetInt("Achievement_Crystals_" + SCENE_NAMES[3], 1);
		PlayerPrefs.Save();
		Debug.Log("GOT ALL CRYSTALS IN ICE");
	}

	static void GameCompleted() {
		SteamManager.Instance.AchieveBeatGame();
		PlayerPrefs.SetInt("Achievement_Complete_Game", 1);
		PlayerPrefs.Save();
		Debug.Log("CLEARED THE GAME!");
	}

	static void GameAllCrystals() {
		SteamManager.Instance.AchieveAllCrystalsInGame();
		PlayerPrefs.SetInt("Achievement_Crystals_Game", 1);
		PlayerPrefs.Save();
		Debug.Log("GOT ALL CRYSTALS IN THE GAME!!!");
	}
}

using UnityEngine;
using System.Collections;
using Facepunch.Steamworks;

public class SteamManager : MonoBehaviour {
	int testState = -1;

	bool initialized = false;

	public static Facepunch.Steamworks.Client SteamClient;
  private ServerList.Request serverRequest;

	public bool grassBeat, desertBeat, iceBeat, gameBeat;
	public bool grassCrystals, desertCrystals, iceCrystals, gameCrystals;

	private static SteamManager s_instance;
	public static SteamManager getInstance() {
		if (s_instance == null) {
			return new GameObject("SteamManager").AddComponent<SteamManager>();
		}
		else {
			return s_instance;
		}
	}

	private static bool s_EverInialized;

	private void Awake() {
		if (s_instance != null) {
			Destroy(gameObject);
			return;
		}
		s_instance = this;

		// We want our SteamManager Instance to persist across scenes.
		DontDestroyOnLoad(gameObject);

		s_EverInialized = true;
	}

	// This should only ever get called on first load and after an Assembly reload, You should never Disable the Steamworks Manager yourself.
	private void OnEnable() {
		if (s_instance == null) {
			s_instance = this;
		}
	}


	private void OnDestroy() {
		if (s_instance != this) {
			return;
		}

		s_instance = null;
	}

	void Start(){
		init();
	}

	void init (){
		if(initialized) return;
		initialized = true;
	    //
      // Configure for Unity
      //
	    Facepunch.Steamworks.Config.ForUnity( Application.platform.ToString() );

      //
      // Create the steam client using Rust's AppId
      //
      SteamClient = new Client( 629420 );

      //
      // Make sure we started up okay
      //
      if ( !SteamClient.IsValid )
      {
          SteamClient.Dispose();
          SteamClient = null;
          return;
      }

      //
      // Request a list of servers
      //
	    {
        serverRequest = SteamClient.ServerList.Internet();
	    }

      var gotStats = false;
      SteamClient.Achievements.OnUpdated += () => { gotStats = true; };

      while ( !gotStats ){
          SteamClient.Update();
      }

	  CheckBeatGrass();
	  CheckBeatDesert();
	  CheckBeatIce();
	  CheckBeatGame();
	  CheckAllCrystalsInGrass();
	  CheckAllCrystalsInDesert();
	  CheckAllCrystalsInIce();
	  CheckAllCrystalsInGame();

	  //ResetGame();
	}

	void Update(){
	  //testFunction();
      if ( SteamClient != null )
      {
          SteamClient.Update();
      }
	}

	private void testFunction(){
		int newState = (int)(Time.time / 3);
		if(newState != testState){
			testState = newState;
			if(testState == 0){
				ResetGame();
				print("achievements reset");
			}else{
				SetAchievement("a"+testState);
				print("a"+testState);
			}
		}
	}

	public string GetSteamName(){
		return SteamClient.Username;
	}

	public void ResetGame(){
		/*
		SteamClient.Achievements.Reset("a1");
		SteamClient.Achievements.Reset("a2");
		SteamClient.Achievements.Reset("a3");
		SteamClient.Achievements.Reset("a4");
		SteamClient.Achievements.Reset("a5");
		SteamClient.Achievements.Reset("a6");
		SteamClient.Achievements.Reset("a7");
		SteamClient.Achievements.Reset("a8");*/
	}

	public bool CheckAllCrystalsInDesert(){
		desertCrystals = GetAchievement("a1");
		return desertCrystals;
	}

	public void AchieveAllCrystalsInDesert(){
		desertCrystals = true;
		SetAchievement("a1");
	}

	public bool CheckAllCrystalsInGame(){
		gameCrystals = GetAchievement("a2");
		return gameCrystals;
	}

	public void AchieveAllCrystalsInGame(){
		gameCrystals = true;
		SetAchievement("a2");
	}

	public bool CheckAllCrystalsInGrass(){
		grassCrystals = GetAchievement("a3");
		return grassCrystals;
	}

	public void AchieveAllCrystalsInGrass(){
		grassCrystals = true;
		SetAchievement("a3");
	}

	public bool CheckAllCrystalsInIce(){
		iceCrystals = GetAchievement("a4");
		return iceCrystals;
	}

	public void AchieveAllCrystalsInIce(){
		iceCrystals = true;
		SetAchievement("a4");
	}

	public bool CheckBeatGame(){
		gameBeat = GetAchievement("a5");
		return gameBeat;
	}

	public void AchieveBeatGame(){
		gameBeat = true;
		SetAchievement("a5");
	}

	public bool CheckBeatDesert(){
		desertBeat = GetAchievement("a6");
		return desertBeat;
	}

	public void AchieveBeatDesert(){
		desertBeat = true;
		SetAchievement("a6");
	}

	public bool CheckBeatIce(){
		iceBeat = GetAchievement("a7");
		return iceBeat;
	}

	public void AchieveBeatIce(){
		iceBeat = true;
		SetAchievement("a7");
	}

	public bool CheckBeatGrass(){
		grassBeat = GetAchievement("a8");
		return grassBeat;
	}

	public void AchieveBeatGrass(){
		grassBeat = true;
		SetAchievement("a8");
	}

	private void SetAchievement(string id){
		init();
			foreach ( var ach in SteamClient.Achievements.All ){
          if(ach.Id == id){
            ach.Trigger();
          }
      }
	}

	private bool GetAchievement(string id){
		init();
		foreach ( var ach in SteamClient.Achievements.All ){
	      if(ach.Id == id){
			return ach.State;
	      }
	    }
		return false;
	}
}

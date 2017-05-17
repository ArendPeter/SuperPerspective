// The SteamManager is designed to work with Steamworks.NET
// This file is released into the public domain.
// Where that dedication is not recognized you are granted a perpetual,
// irrevokable license to copy and modify this file as you see fit.
//
// Version: 1.0.5

using UnityEngine;
using System.Collections;
using Facepunch.Steamworks;

public class SteamManager : MonoBehaviour {
		public static Facepunch.Steamworks.Client SteamClient;
    private ServerList.Request serverRequest;

    void Start (){
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

        while ( !gotStats )
        {
            SteamClient.Update();
        }

				testFunction();
		}

		void Update()
    {
        if ( SteamClient != null )
        {
            SteamClient.Update();


        }
    }

	private void testFunction(){
		Debug.Log(GetSteamName());
		print("all desert crystals?");
		print(GetAchievement("a3"));
		print("all crystals in game?");
		print(GetAchievement("a2"));
		print("setting all desert crystals to true");
		AchieveAllCrystalsInGrass();
		print("all desert crystals?");
		print(GetAchievement("a3"));
	}

	public string GetSteamName(){
		return SteamClient.Username;
	}

	public bool CheckAllCrystalsInDesert(){
		return GetAchievement("a1");
	}

	public void AchieveAllCrystalsInDesert(){
		SetAchievement("a1");
	}

	public bool CheckAllCrystalsInGame(){
		return GetAchievement("a2");
	}

	public void AchieveAllCrystalsInGame(){
		SetAchievement("a2");
	}

	public bool CheckAllCrystalsInGrass(){
		return GetAchievement("a3");
	}

	public void AchieveAllCrystalsInGrass(){
		SetAchievement("a3");
	}

	public bool CheckAllCrystalsInIce(){
		return GetAchievement("a4");
	}

	public void AchieveAllCrystalsInIce(){
		SetAchievement("a4");
	}

	public bool CheckBeatGame(){
		return GetAchievement("a5");
	}

	public void AchieveBeatGame(){
		SetAchievement("a5");
	}

	public bool CheckBeatDesert(){
		return GetAchievement("a6");
	}

	public void AchieveBeatDesert(){
		SetAchievement("a6");
	}

	public bool CheckBeatIce(){
		return GetAchievement("a7");
	}

	public void AchieveBeatIce(){
		SetAchievement("a7");
	}

	public bool CheckBeatForest(){
		return GetAchievement("a8");
	}

	public void AchieveBeatForest(){
		SetAchievement("a8");
	}

	public void SetAchievement(string id){
			foreach ( var ach in SteamClient.Achievements.All ){
          if(ach.Id == id){
            ach.Trigger();
          }
      }
	}

	public bool GetAchievement(string id){
			foreach ( var ach in SteamClient.Achievements.All ){
          if(ach.Id == id){
							return ach.State;
          }
      }
			return false;
	}
}

using UnityEngine;
using System.Collections;

public class Door : ActiveInteractable {

	//public Door dest;
	public string myName;
	public string destName;
	Door destDoor;
    //public Color particleColor;

    public ParticleSystem[] teleparticle;
    public bool isParticleActive = true;
    private float distanceCutoff = 90f;
    public float currentDistance = 0;

	public string sceneName;
	public int crystalRequirement;

	public Vector3 teleportOffset = new Vector3(0,3,0);
    public GameObject warpSound;

    private AdviceGiver AG;

    ListenerHandler l;

	public void Awake(){
		//update particle color
		l = GameObject.Find("AudioListener").GetComponent<ListenerHandler>();


        //ParticleSystem p = this.transform.FindChild("Particles").GetComponent<ParticleSystem>();
        //teleparticle = this.transform.FindChild("Particles").GetComponent<ParticleSystem>();
        teleparticle = this.GetComponentsInChildren<ParticleSystem>();
        /*if(p != null){
			p.startColor = particleColor;
			p.Simulate(2f);
			p.Play();
		}*/

        range = 2;

	    if (this.invisible) {
            MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mr in children) {
            	mr.enabled = false;
            }

            ParticleSystem[] particleChildren = GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem ps in particleChildren) {
            	ps.Stop();
            }
        }
	}

    void Update()
    {
        currentDistance = GetDistance();
        if (!invisible)
        {
            if (GetDistance() > distanceCutoff)
            {
                foreach (ParticleSystem ps in teleparticle)
                {
                    ps.Stop();
                    isParticleActive = false;
                }

            }
            else
            {
                if (!isParticleActive)
                {
                    foreach (ParticleSystem ps in teleparticle)
                    {
                        isParticleActive = true;
                        ps.Play();
                    }
                }
            }
        }
        //print(currentDistance);
    }

	public static void TeleportPlayerToDoor(PlayerController p, string doorName) {
		Door dest = DoorManager.instance.getDoor(doorName);
		p.Teleport(
			dest.GetComponent<Collider>().bounds.center + dest.teleportOffset);
		Instantiate(dest.warpSound);
        /*if (dest.teleparticle != null)
        {
            //dest.teleparticle.Emit(700);
        }*/
	}

	public override float GetDistance() {
		if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
			return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z),
			                        new Vector2(transform.position.x, transform.position.z));
		else
			return Mathf.Abs(player.transform.position.x - transform.position.x);
	}

	protected override bool isPlayerFacingObject() {
		Vector3 pPos = player.transform.position;
		Bounds bounds = GetComponent<Collider>().bounds;
		if (pPos.x >= bounds.min.x && pPos.x <= bounds.max.x && pPos.z >= bounds.min.z && pPos.z <= bounds.max.z) {
			return true;
		}

		return base.isPlayerFacingObject();
	}

	protected override bool isPlayerFacingObject2D(){
		Vector3 pPos = player.transform.position;
		Bounds bounds = GetComponent<Collider>().bounds;
		if(bounds.min.x <= pPos.x && pPos.x <= bounds.max.x){
			return true;
		}
		return base.isPlayerFacingObject2D();
	}

	public override void Triggered(){
		//Creates game object that plays warp sound then self destructs -Nick
        Instantiate(warpSound);
		if (myName.Equals("hub-end")) {
			PlayerPrefs.SetInt("GameComplete", 1);
			PlayerPrefs.Save();
			AchievementManager.CheckAchievements();
		}

		if(sceneName != "") {
			TransitionManager.instance.SceneTransition(player.GetComponent<PlayerController>(), destName, sceneName);
		}else if(destDoor!=null && MainCollectable.GetMainCollectableHeld() >= crystalRequirement){

            if (AG == null)
            {
                AG = GameObject.FindObjectOfType<AdviceGiver>();
            }
            //print("Teleport to " + destName);
            AG.currentLoc = destName;

            player.GetComponent<PlayerController>().Teleport(
				destDoor.GetComponent<Collider>().bounds.center + teleportOffset);

			// Saving level progress
			string level = Application.loadedLevelName;
			string doorsFound = PlayerPrefs.GetString(level);
			if (destName.Contains("start")) {
				if (doorsFound.Equals("")) {
					doorsFound += destDoor.getName();
					PlayerPrefs.SetString(level, doorsFound);
					PlayerPrefs.Save();
					Saved_UI.instance.ShowSaved();
				} else if (!doorsFound.Contains(destDoor.getName())) {
					doorsFound += "," + destDoor.getName();
					PlayerPrefs.SetString(level, doorsFound);
					PlayerPrefs.Save();
					Saved_UI.instance.ShowSaved();
				}
			}
		}else{
			//Debug.Log("Door not linked");
		}
    l.ResetZ();
  }

	public string getName(){
		return myName;
	}

	public void setDoor(Door destDoor){
		this.destDoor = destDoor;
	}

	protected override bool IsInYRange(){
		float playerY = PlayerController.instance.transform.position.y;
		float myY = transform.position.y;
		float deltaY = playerY - myY;
		return -.5f < deltaY && deltaY < 4f;
	}

	protected override bool IsEnabled(){
		if(PlayerController.instance.isRiding()){
			return false;
		}else{
			return base.IsEnabled();
	 	}
	}
}

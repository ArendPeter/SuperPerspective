using UnityEngine;
using System.Collections;

public class Crate : ActiveInteractable {

	#pragma warning disable 219, 414

	private const float gravity = 1.5f;
	private const float terminalVelocity = 60;
	private const float decelleration = 1;

	private Vector3 trajectory, newVelocity;
	private bool grounded, svFlag, startFalling;
	private float colliderHeight, colliderWidth, colliderDepth;
	private float Margin = .05f;

	private Vector3 startPos;

	private CollisionChecker colCheck;

	private bool grabbed, fpFlag, respawnFlag = false;

	private PerspectiveType persp = PerspectiveType.p3D;

	private bool[] axisBlocked = new bool[4];

	public GameObject brokenCrateSpawnPoint;
	public GameObject brokenCrate;
	public GameObject spawnCircle;

	float verticalOverlapThreshhold = .3f;

	float groundY = 0f;

	void Start() {
		base.StartSetup ();
		grounded = false;
		startFalling = false;
		colliderHeight = GetComponent<Collider>().bounds.size.y;
		colliderWidth = GetComponent<Collider>().bounds.size.x;
		colliderDepth = GetComponent<Collider>().bounds.size.z;

		//player = GameObject.Find("Player").GetComponent<PlayerController>();

		// Register CheckGrab to grab input event
		//InputManager.instance.InteractPressed += CheckGrab;
		GameStateManager.instance.PerspectiveShiftEvent += Shift;
		CameraController.instance.TransitionEndingEvent += checkBreak;
		colCheck = new CollisionChecker (GetComponent<Collider> ());
		startPos = transform.position;

		for (int i = 0; i < 4; i++)
			axisBlocked[i] = false;

		findGroundY();
	}

	private void findGroundY(){
		GameObject groundObj = IslandControl.instance.findGround(gameObject);
		groundY = groundObj.transform.position.y;
		float myHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		float groundHeight = groundObj.GetComponent<Collider>().bounds.max.y - groundObj.GetComponent<Collider>().bounds.min.y;
		groundY += (myHeight + groundHeight) * .5f;
	}

	void Update() {
		if(!PlayerController.instance.isPaused()){
			if (grabbed) {
				if (GetDistance() > range * (1 + Margin) || PlayerController.instance.isFalling()) {
					player.GetComponent<PlayerController>().Grab(null);
					grabbed = false;
				}
			}
		}
		stayAboveGround();
	}

	private void stayAboveGround(){
		Vector3 pos = transform.position;
		if(pos.y < groundY && velocity.y < -.01f){
			pos.y = groundY;
			velocity.y = 0f;
			transform.position = pos;
		}
	}

	void FixedUpdate() {
		if(!PlayerController.instance.isPaused()){
			base.FixedUpdateLogic ();
			if (!grounded)
				velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - gravity, -terminalVelocity), velocity.z);

			/*if (grabbed) {
				float vy = velocity.y;
				velocity = player.GetComponent<PlayerController>().GetVelocity();
				velocity.y = vy;
			}*/

			//CheckCollisions();

			if (!svFlag && !fpFlag && !grabbed) {
				float newVelocityX = velocity.x, newVelocityZ = velocity.z;
				if (velocity.x != 0)
				{
					int modifier = velocity.x > 0 ? -1 : 1;
					newVelocityX += Mathf.Min(decelleration, Mathf.Abs(velocity.x)) * modifier;
				}
				velocity.x = newVelocityX;

				if (velocity.z != 0)
				{
					int modifier = velocity.z > 0 ? -1 : 1;
					newVelocityZ += Mathf.Min(decelleration, Mathf.Abs(velocity.z)) * modifier;
				}
				velocity.z = newVelocityZ;

				if (GetComponent<Collider> ().enabled) {
					colliderHeight = GetComponent<Collider>().bounds.size.y;
					colliderWidth = GetComponent<Collider>().bounds.size.x;
					colliderDepth = GetComponent<Collider>().bounds.size.z;
				}
			} else {
				velocity.x = newVelocity.x;
				velocity.z = newVelocity.z;
				svFlag = false;
				fpFlag = false;
			}

			CheckCollisions();

			//Adding in pushing sound, initialize after break sound -Nick

			//Init
			if (gameObject.GetComponent<AudioSource> ().clip.name != "CratePush" && !respawnFlag && grounded) {
				gameObject.GetComponent<AudioSource> ().clip =  Resources.Load ("Sound/SFX/Objects/Box/CratePush")  as AudioClip;
				gameObject.GetComponent<AudioSource> ().loop = true;
				gameObject.GetComponent<AudioSource>().volume = 0;
				gameObject.GetComponent<AudioSource>().Play ();

			}

			//Check
			if (velocity.magnitude > 0.1f && grounded){
				if(gameObject.GetComponent<AudioSource>().volume < 1){
					gameObject.GetComponent<AudioSource>().volume += 0.5f;
				}
			}
			else{
				gameObject.GetComponent<AudioSource>().volume = 0;
			}

			//End Nick stuff
		}
	}

	void LateUpdate () {
		if (!GetComponent<LevelGeometry>().foundPlatform()) {
			return;
		}
		if(!PlayerController.instance.isPaused()){
			base.LateUpdateLogic ();
			transform.Translate(velocity * Time.deltaTime);
			bool playerAwayFromSpawn =
			 	Vector2.Distance(new Vector2(startPos.x, startPos.y), new Vector2(player.transform.position.x, player.transform.position.y)) > colliderWidth;
			if (respawnFlag && playerAwayFromSpawn) {
				Vector3 pos = transform.position;
				pos = startPos + Vector3.up;
				transform.position = pos;
				GetComponent<Collider>().enabled = true;
				GetComponentInChildren<Renderer>().enabled = true;
				//SPAWN A SPAWN CIRCLE
				if(spawnCircle != null){
					GameObject.Instantiate(spawnCircle, transform.position, Quaternion.identity);
				}
				GetComponent<LevelGeometry>().AdjustPosition(GameStateManager.instance.currentPerspective);
				respawnFlag = false;
			}
			//CheckCollisions();
		}
	}

	public void CheckCollisions() {
		Vector3 trajectory;

		RaycastHit[] hits = colCheck.CheckYCollision (velocity, Margin);

		for (int i = 0; i < 4; i++)
			axisBlocked[i] = false;

		float close = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];
			if (hitInfo.collider != null)
			{
				if (hitInfo.collider.gameObject.tag == "Intangible") {
					trajectory = velocity.y * Vector3.up;
					CollideWithObject(hitInfo, trajectory);
				} else if (close == -1 || close > hitInfo.distance) {
					close = hitInfo.distance;
					if (velocity.y < 0) {
						grounded = true;
					}
					trajectory = velocity.y * Vector3.up;
					CollideWithObject(hitInfo, trajectory);
				}
			}
		}
		if (close == -1) {
			grounded = false;
		} else {
			transform.Translate(Vector3.up * Mathf.Sign(velocity.y) * (close - colliderHeight / 2));
			velocity = new Vector3(velocity.x, 0f, velocity.z);
		}

		if (velocity.x != 0){
			// Third check the player's velocity along the X axis and check for collisions in that direction is non-zero

			// If any rays connected move the player and set grounded to true since we're now on the ground

			hits = colCheck.CheckXCollision (velocity, Margin);

			close = -1;
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hitInfo = hits[i];
				if (hitInfo.collider != null)
				{
					float verticalOverlap = getVerticalOverlap(hitInfo);
					bool significantVerticalOverlap =
						verticalOverlap > verticalOverlapThreshhold;
					if(!significantVerticalOverlap){
						transform.Translate(new Vector3(0f,verticalOverlap,0f));
						continue;
					}
					if (hitInfo.collider.gameObject.tag == "Intangible") {
						trajectory = velocity.x * Vector3.right;
						CollideWithObject(hitInfo, trajectory);
					} else if (close == -1 || close > hitInfo.distance) {
						close = hitInfo.distance;
						transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (hitInfo.distance - colliderWidth / 2));
						trajectory = velocity.x * Vector3.right;
						if (trajectory != Vector3.zero)
							axisBlocked[HashAxis(trajectory)] = true;
						CollideWithObject(hitInfo, trajectory);
					}
				}
			}
			if (close != -1) {
				//transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (close - colliderWidth / 2));
				velocity = new Vector3(0f, velocity.y, velocity.z);
			}
		}

		if (velocity.z != 0){
			// Fourth do the same along the Z axis

			// If any rays connected move the player and set grounded to true since we're now on the ground
			hits = colCheck.CheckZCollision (velocity, Margin);

			close = -1;
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hitInfo = hits[i];
				if (hitInfo.collider != null)
				{
					float verticalOverlap = getVerticalOverlap(hitInfo);
					bool significantVerticalOverlap =
						verticalOverlap > verticalOverlapThreshhold;
					if(!significantVerticalOverlap){
						transform.Translate(new Vector3(0f,verticalOverlap,0f));
						continue;
					}
					if (hitInfo.collider.gameObject.tag == "Intangible") {
						trajectory = velocity.z * Vector3.forward;
						CollideWithObject(hitInfo, trajectory);
					} else if (close == -1 || close > hitInfo.distance) {
						close = hitInfo.distance;
						transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (hitInfo.distance - colliderDepth / 2));
						trajectory = velocity.z * Vector3.forward;
						if (trajectory != Vector3.zero)
							axisBlocked[HashAxis(trajectory)] = true;
						CollideWithObject(hitInfo, trajectory);
					}
				}
			}
			if (close != -1) {
				//transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (close - colliderDepth / 2));
				velocity = new Vector3(velocity.x, velocity.y, 0f);
			}
		}
	}

	public bool Check2DIntersect() {
		// True if any ray hits a collider
		bool connected = false;

		//reference variables
		float minX 		= GetComponent<Collider>().bounds.min.x + Margin;
		float centerX 	= GetComponent<Collider>().bounds.center.x;
		float maxX 		= GetComponent<Collider>().bounds.max.x - Margin;
		float minY 		= GetComponent<Collider>().bounds.min.y + Margin;
		float centerY 	= GetComponent<Collider>().bounds.center.y;
		float maxY 		= GetComponent<Collider>().bounds.max.y - Margin;
		float centerZ   = GetComponent<Collider>().bounds.center.z;
		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, maxY, centerZ),
			new Vector3(maxX, maxY, centerZ),
			new Vector3(minX, minY, centerZ),
			new Vector3(maxX, minY, centerZ),
			new Vector3(centerX, centerY, centerZ)
		};

		// Was there a point to this? It only seems to mess it up in some cases
		//Vector3 side = new Vector3(0,0,colliderDepth/2 - Margin);

		//check all startpoints
		for (int i = 0; i < startPoints.Length; i++) {
			RaycastHit hit;
			connected = connected ||
			Physics.Raycast (startPoints [i], Vector3.forward) ||
			Physics.Raycast (startPoints [i], -Vector3.forward);
		}
		return connected;
	}

	void checkBreak() {
		if(GameStateManager.is2D() && !GameStateManager.isFailedShift() && Check2DIntersect()) {
			doBreak();
		}
	}

	void doBreak() {
		setVelocity(Vector3.zero);
		if (grabbed){
			player.GetComponent<PlayerController> ().Grab (null);
			grabbed = false;
		}
		respawnFlag = true;
		playBreakSound();
	}

	private void playBreakSound(){
		gameObject.GetComponent<AudioSource>().loop = false;
		gameObject.GetComponent<AudioSource>().Stop ();
		gameObject.GetComponent<AudioSource>().clip = Resources.Load ("Sound/SFX/Objects/Box/CrateBreak")  as AudioClip;
		gameObject.GetComponent<AudioSource>().volume = 1;
		gameObject.GetComponent<AudioSource>().Play();
	}

	// Used to check collisions with special objects
	// Make this more object oriented? Collidable interface?
	private void CollideWithObject(RaycastHit hitInfo, Vector3 trajectory) {
		GameObject other = hitInfo.collider.gameObject;
		float colliderDim = 0;
		if (trajectory.normalized == Vector3.up || trajectory.normalized == Vector3.down)
			colliderDim = colliderHeight;
		if (trajectory.normalized == Vector3.right || trajectory.normalized == Vector3.left)
			colliderDim = colliderWidth;
		if (trajectory.normalized == Vector3.forward || trajectory.normalized == Vector3.back)
			colliderDim = colliderDepth;
        // Pressure Plate
		if (other.GetComponent<PushSwitchOld>() && colliderDim == colliderWidth) {
			transform.Translate(0, 0.1f, 0);
		}
		// Fall on Player/Moving Platform
		if ((other.GetComponent<MobilePlatform>() || other.GetComponent<PlayerController>()) && colliderDim == colliderHeight) {
			if ((other.GetComponent<MobilePlatform>() && other.GetComponent<MobilePlatform>().controlled) ||
				(other.GetComponent<PlayerController>() && other.GetComponent<PlayerController>().isRiding()))
				doBreak();
		}
		//Collision w/ PlayerInteractable
		foreach(Interactable c in other.GetComponents<Interactable>()){
			c.EnterCollisionWithGeneral(gameObject);
		}
	}

	private float getVerticalOverlap(RaycastHit hitInfo){
		Collider hitCollider = hitInfo.collider;
		float hitColliderHeight = hitCollider.bounds.max.y - hitCollider.bounds.min.y;
		float myBottomY = GetComponent<Collider>().bounds.min.y;
		float hitTopY = hitCollider.bounds.max.y;
		float overlap = hitTopY - myBottomY;
		return overlap;
	}

	public override void Triggered() {
		if (!grabbed && !PlayerController.instance.isFalling()) {
			player.GetComponent<PlayerController> ().Grab (this);
			grabbed = true;
		} else {
			player.GetComponent<PlayerController> ().Grab (null);
			grabbed = false;
		}
	}

	public void SetVelocity(float x, float z) {
		newVelocity.x = x;
		newVelocity.z = z;
		svFlag = true;
	}

	public void FreePush(float x, float z) {
		SetVelocity (x, z);
		fpFlag = true;
	}

	public Vector3 GetVelocity() {
		return velocity;
	}

	public bool IsGrounded() {
		return grounded;
	}

	public bool IsAxisBlocked(Vector3 axis) {
		if (HashAxis(axis) != -1)
			return axisBlocked[HashAxis(axis)];
		return true;
	}

	public int HashAxis(Vector3 axis) {
		if (axis.normalized == Vector3.right) {
			return 0;
		} else if (axis.normalized == Vector3.left) {
			return 1;
		} else if (axis.normalized == Vector3.forward) {
			return 2;
		} else if (axis.normalized == Vector3.back) {
			return 3;
		}
		return -1;
	}

	private void Shift(PerspectiveType p) {
		persp = p;
		if (respawnFlag) {
			GetComponent<Collider>().enabled = false;
			GetComponentInChildren<Renderer>().enabled = false;
			if(brokenCrate != null){
				GameObject.Instantiate(brokenCrate, brokenCrateSpawnPoint.transform.position, Quaternion.identity);
			}
		}
	}
}

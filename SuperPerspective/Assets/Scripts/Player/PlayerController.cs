using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PlayerController : PhysicalObject{
	//suppress warnings
	#pragma warning disable 1691,168,219,414

   #region Properties & Variables

	//singleton
	public static PlayerController instance;

	//animator
	private PlayerAnimController animator;

	// Movement Parameters
	public float acceleration;
	public float decelleration;
	public float maxSpeed;
	public float hangMaxSpeed;
	public float upGravity;
	public float downGravity;
	public float terminalVelocity;
	public float jumpVelocity;
	public float jumpMargin;
	public float cactusOutwardVelocity;
	public float cactusVerticalVelocity;

	// Verticle movement flags
	private bool jumpPressedOnPreviousFrame;
	private bool bounced;
	private float jumpPressedTime;
	private bool jumping;
	private bool grounded;

	// Raycasting Variables
	public int verticalRays = 8;
	float Margin = 0.05f;
	float verticalOverlapThreshhold = .3f;

	private Rect box;
	private float colliderHeight;
	private float colliderWidth;
	private float colliderDepth;

	private CollisionChecker colCheck;

	// Vars for Z-locking
	private float zlock = int.MinValue;
	private bool zlockFlag;

	private Crate crate = null;
	private bool passivePush = false;
	private Vector3 grabAxis = Vector3.zero;
	private bool launched;
	private GameObject ridingPlatform = null;
	private Orb orb = null;

	//Vars for edge grabbing
	private Vector3[] cuboid;
	private Edge grabbedEdge = null;
	private PlayerEdgeState edgeState = PlayerEdgeState.FAR_FROM_EDGE;

	private int climbTimer = 0;
	private const int CLIMB_TIME = 10;

	private int kickTimer = 0;
	private const int KICK_TIME = 10;

	private int cactusKnockBackTimer = 0;
	private const int CACTUS_TIME = 20;

	private bool cutsceneMode = false;
	private float cutsceneModeDisableTime = 0f;

	private bool _paused;

	private const int X = 0;
	private const int Y = 1;
	private const int Z = 2;
	private const float epsilon = .1f;

	float frameRate;
	public float testFrameRate;

    GameObject hoodieGirl;

    RaycastHit hit;
    public LayerMask mask;

    NotificationController nCont;

    public bool canInteract = false;

    //Nick addition, used for footstep and voice sound selection
    public StepManager step;
    public VoiceManager voice;

    #endregion

	#region Init

	void Awake(){
		generateSingleton();
	}

	void Start () {
		base.Init();

		linkAnimator();

		initMovementVariables();

		initCollisionVariables();

		registerEventHandlers();

        //Nick addition, used for footstep and voice sound selection
        step = GameObject.Find("Steps").GetComponent<StepManager>();
        voice = GameObject.Find("Voice").GetComponent<VoiceManager>();

        //Used to obtain player direction for traditional raycasting collision -Nick
        hoodieGirl = FindObjectOfType<playerControllerProxy>().gameObject;
        nCont = FindObjectOfType<NotificationController>();
        InputManager.instance.InteractPressedEvent += OnInteract;

        //PlayerPrefs.DeleteAll();
    }

	public void Reset() {
		initMovementVariables();
	}

	private void generateSingleton(){
		if(instance == null)
			instance = this;
		else if(instance!= this)
			Destroy(this);
	}

	private void linkAnimator(){
		animator = PlayerAnimController.instance;
	}

	private void initMovementVariables(){
		grounded = false;
		jumpPressedOnPreviousFrame = false;
	}

	private void initCollisionVariables(){
		cuboid = new Vector3[2];
		colliderHeight = GetComponent<Collider>().bounds.max.y - GetComponent<Collider>().bounds.min.y;
		colliderWidth = GetComponent<Collider>().bounds.max.x - GetComponent<Collider>().bounds.min.x;
		colliderDepth = GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.min.z;
		colCheck = new CollisionChecker(GetComponent<Collider> ());
	}

	private void registerEventHandlers(){
		GameStateManager.instance.GamePausedEvent += OnPauseGame;
    }

	#endregion Init

	void Update(){
		frameRate = 1 / Time.deltaTime;
		testFrameRate = frameRate;
		if(canControl()) checkForJump();
        if (hoodieGirl != null)
        {
            CheckForInteractibleRaycast();
        }
	}

    private void OnInteract()
    {
        if (canInteract)
        {
            hit.collider.gameObject.GetComponent<GreatCrystalWall>().Activate();
        }
    }

    void CheckForInteractibleRaycast()
    {
        if (Physics.Raycast(hoodieGirl.transform.position, hoodieGirl.transform.forward, out hit, 1.5f, mask))
        {
            if (hit.collider != null && hit.collider.gameObject.GetComponent<GreatCrystalWall>() != null)
            {
                canInteract = true;
                return;
            }
        }
        canInteract = false;
    }

	#region Jump
	private void checkForJump(){
		updateJumpPressedTime();

		bool jumpInputed = (Time.time - jumpPressedTime) < jumpMargin;
		bool canJump = (isGrounded() || edgeState == PlayerEdgeState.HANGING) && !GrabbedCrate();
		if(canJump && jumpInputed)
			jump();
	}

	private void updateJumpPressedTime(){
		bool jumpPressedThisFrame = InputManager.instance.JumpStatus();

		if (jumpPressedThisFrame && !jumpPressedOnPreviousFrame){
			jumpPressedTime = Time.time;
		}else if (!jumpPressedThisFrame){
			jumpPressedTime = 0;
		}

		jumpPressedOnPreviousFrame = jumpPressedThisFrame;
	}

	private void jump(){
		grounded = false;
		velocity = new Vector3(velocity.x,
			Mathf.Max(velocity.y,jumpVelocity), velocity.z);
		jumpPressedTime = 0;
		jumping = true;

		ReleaseEdge();
	}

	#endregion Jump

	// Collision detection and velocity calculations are done in the fixed update step
	void FixedUpdate(){
		updateTimers();
		updateCuboid();

		if(canControl()) move();

		if(!isDisabled()) updateStateVariables();

		if (frameRate <= 50) {
			if(canMove()) applyGravity();

			CheckCollisions();

			if(canMove()) applyMovement();
		}
	}

	private void updateTimers(){
		if(kickTimer > 0) kickTimer--;
		if(climbTimer > 0) climbTimer--;
		if(cactusKnockBackTimer > 0) cactusKnockBackTimer--;
	}

	private void updateCuboid(){
		Vector3 halfScale = gameObject.transform.lossyScale * .5f;
		cuboid[0] = gameObject.transform.position - halfScale;
		cuboid[1] = gameObject.transform.position + halfScale;
	}

	private void move(){
		if(edgeState == PlayerEdgeState.HANGING){
			bool edgeOnXAxis = grabbedEdge.getOrientation() % 2 == 1;
			if(edgeOnXAxis)
				shimmyOnAxis(X);
			else
				shimmyOnAxis(Z);
		}else{
			moveOnAxis(X);
			moveOnAxis(Z);
		}
	}

	private void moveOnAxis(int axis){
		float newVelocity = velocity[axis];

		float axisInput = (axis == X)?
			InputManager.instance.GetForwardMovement():
			-InputManager.instance.GetSideMovement();

		if (axisInput != 0){
			newVelocity += acceleration * Mathf.Sign(axisInput);
			newVelocity = Mathf.Clamp(newVelocity,
				-maxSpeed * Mathf.Abs(axisInput),
				maxSpeed * Mathf.Abs(axisInput)
			);
		}else if (velocity[axis] != 0){
			newVelocity -= decelleration * Mathf.Sign(newVelocity);
			if(Mathf.Sign(newVelocity) != Mathf.Sign(velocity[axis])) newVelocity = 0;
		}

		velocity[axis] = newVelocity;
	}

	private void shimmyOnAxis(int axis){
		float axisInput = (axis == X)?
			InputManager.instance.GetForwardMovement():
			-InputManager.instance.GetSideMovement();

		float newVelocity = hangMaxSpeed * Mathf.Sign(axisInput);

		Vector3 pos = transform.position;
		pos[axis] += newVelocity/50f;

		if(axisInput == 0 || !grabbedEdge.isPositionValidOnEdge(pos))
			newVelocity = 0;

		velocity[axis] = newVelocity;
	}

	private void updateStateVariables(){
		if(isFalling()) jumping = false;
	}

	#region Collisions

	public void CheckCollisions(){
		CheckCollisionsOnAxis(Y);
		passivePush = false;
		CheckCollisionsOnAxis(X);
		CheckCollisionsOnAxis(Z);
	}

	void CheckCollisionsOnAxis(int axis){
		Vector3 axisVector = getAxisVector(axis);

		Vector3 trajectory;

		RaycastHit[] hits = colCheck.CheckCollisionOnAxis(axis,velocity, Margin);

		float distToCollision = -1;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hitInfo = hits[i];

	    	if (hitInfo.collider != null && !hitInfo.collider.isTrigger)
			{

	      		//Nick code: call to StepManager method that uses type of y axis collision to chage footstep sound

		        if (axis == Y)
		        {
		            if (step != null)
		                step.updateStepType(hitInfo.collider);
		        }

		        //end Nick code

		        float verticalOverlap = getVerticalOverlap(hitInfo);
				bool significantVerticalOverlap =
				verticalOverlap > verticalOverlapThreshhold;
				if(axis != Y && !significantVerticalOverlap){
					transform.Translate(new Vector3(0f,verticalOverlap,0f));
					continue;
				}
				if (hitInfo.collider.gameObject.tag == "Intangible") {
					trajectory = velocity[axis] * axisVector;
					CollideWithObject(hitInfo, trajectory);
				} else if (distToCollision == -1 || distToCollision > hitInfo.distance) {
					distToCollision = hitInfo.distance;
					if(axis == Y){
						if (velocity.y < 0) {
							grounded = true;
							launched = false;
							// New Z-lock
							if (hitInfo.collider.gameObject.tag != "Ground" && GameStateManager.instance.currentPerspective == PerspectiveType.p2D) {
								Vector3 pos = transform.position;
								pos.z = hitInfo.collider.gameObject.transform.position.z;
								transform.position = pos;
							}
						}
		        // Z-lock
		        if (hitInfo.collider.gameObject.GetComponent<LevelGeometry>())
		            zlock = hitInfo.transform.position.z;
		          else
		            zlock = int.MinValue;
					}else{
						transform.Translate(
							axisVector *
							Mathf.Sign(velocity[axis]) *
							(hitInfo.distance - getDimensionAlongAxis(axis) / 2)
						);
					}
					trajectory = velocity[axis] * axisVector;
					CollideWithObject(hitInfo, trajectory);
				}
			}
		}


		bool collisionWithTangibleOccurred = distToCollision!=-1;
		if (collisionWithTangibleOccurred) {
			if (isRiding()) {
				Vector3 pos = transform.position;
				ridingPlatform.transform.Translate(ridingPlatform.GetComponent<MobilePlatform>().getVelocity() * -Time.deltaTime);
				pos.x = ridingPlatform.transform.position.x;
				pos.y = ridingPlatform.transform.position.y + colliderHeight / 2f + ridingPlatform.GetComponent<Collider>().bounds.extents.y - 0.1f;
				pos.z = ridingPlatform.transform.position.z;
				transform.position = pos;
				ridingPlatform.GetComponent<MobilePlatform>().setVelocity(Vector3.zero);
			} else {
				if(axis == Y){
					if (!bounced) {
						transform.Translate(
							axisVector *
							Mathf.Sign(velocity[axis]) *
							(distToCollision - getDimensionAlongAxis(axis) / 2)
						);
						velocity[axis] = 0f;
					} else {
						bounced = false;
					}
				}else{
					if(!passivePush && !isInCactusKnockBack()) velocity[axis] = 0f;
				}
			}
		} else if (axis == Y) {
			grounded = false;
		}
	}

    //Nick addition: Used for footstep audio selection

    void OnCollisionEnter(Collision collider)
    {
        //Debug.Log("colEnter");
        //step.updateStepType(collider);
    }

    //End Nick addition

    Vector3 getAxisVector(int axis){
		switch(axis){
			case X: return Vector3.right;
			case Y: return Vector3.up;
			case Z: return Vector3.forward;
			default:
				throw new System.ArgumentException("Invalid Axis Index");
		}
	}

	float getDimensionAlongAxis(Vector3 axis){
		Vector3 norm = axis.normalized;
		if(Mathf.Abs(norm.x) == 1) return colliderWidth;
		if(Mathf.Abs(norm.y) == 1) return colliderHeight;
		if(Mathf.Abs(norm.z) == 1) return colliderDepth;
		throw new System.ArgumentException("Input Vector must only have values along one axis");
	}

	//Methods related to Insignificant Ovelap Adjustments
	private float getVerticalOverlap(RaycastHit hitInfo){
		Collider hitCollider = hitInfo.collider;
		float hitColliderHeight = hitCollider.bounds.max.y - hitCollider.bounds.min.y;
		float myBottomY = GetComponent<Collider>().bounds.min.y;
		float hitTopY = hitCollider.bounds.max.y;
		float overlap = hitTopY - myBottomY;
		return overlap;
	}

	//Checks Type of Object and collides accordingly
	private void CollideWithObject(RaycastHit hitInfo, Vector3 trajectory) {
		if (isRiding())
			return;
		GameObject other = hitInfo.collider.gameObject;
		float colliderDim = 0;
		if (trajectory.normalized == Vector3.up || trajectory.normalized == Vector3.down)
			colliderDim = colliderHeight;
		if (trajectory.normalized == Vector3.right || trajectory.normalized == Vector3.left)
			colliderDim = colliderWidth;
		if (trajectory.normalized == Vector3.forward || trajectory.normalized == Vector3.back)
			colliderDim = colliderDepth;
		// Bounce Pad
		if (trajectory.normalized == Vector3.down) {
			if (other.GetComponent<BouncePad>()) {
				velocity = other.transform.up * other.GetComponent<BouncePad>().GetBouncePower();
				if (!other.transform.up.Equals(Vector3.up))
					launched = true;
				other.GetComponent<BouncePad>().Animate();
				bounced = true;
			}
			foreach (LandOnObject c in other.GetComponents<LandOnObject>()) {
				c.LandedOn ();
			}
		}
		// Crate
		// first bit was to fix crates moving inside player when you hit another crate while pulling
		if ((!GrabbedCrate() || crate.gameObject.Equals(other.gameObject)) && trajectory.normalized != Vector3.down && trajectory.normalized != Vector3.zero &&
				other.GetComponent<Crate>() && !other.GetComponent<Crate>().IsAxisBlocked(trajectory)) {
			other.GetComponent<Crate>().FreePush((trajectory*0.75f).x, (trajectory*0.75f).z);
			passivePush = true;
		}
		// PushSwitchOld
		if (other.GetComponent<PushSwitchOld>() && colliderDim == colliderWidth) {
			transform.Translate(0, 0.1f, 0);
		}
		// Cactus
		if (!isInCactusKnockBack() && other.tag == "Cactus"){
            voice.Ow();
			cactusKnockBackTimer = CACTUS_TIME;
			Vector2 cacPos = new Vector2(other.transform.position.x,other.transform.position.z);
			Vector2 playerPos = new Vector2(transform.position.x,transform.position.z);
			float outDir = Vector2.Angle(Vector2.right,playerPos-cacPos);
			int zDir = (cacPos.y < playerPos.y)? 1 : -1;
			Vector2 outVec = new Vector2(
				Mathf.Cos(Mathf.Deg2Rad * outDir),zDir * Mathf.Sin(Mathf.Deg2Rad * outDir));
			velocity = new Vector3(
				cactusOutwardVelocity * outVec.x,
			 	cactusVerticalVelocity,
				cactusOutwardVelocity * outVec.y * (GameStateManager.is3D()?1:0));
			if(orb != null){
				orb.SetOutwardDropVector(outVec);
				DropOrb();
			}
		}
		//Collision w/ PlayerInteractable
		foreach (Interactable c in other.GetComponents<Interactable>()) {
			c.EnterCollisionWithPlayer ();
		}
	}

	public void DropOrb(){
			if(orb != null){
				orb.Drop();
				orb = null;
			}
	}

	#endregion Collisions

	// LateUpdate is used to actually move the position of the player
	void LateUpdate () {
		if (frameRate > 50f) {
			if(canMove()) applyGravity();

			CheckCollisions();

			if(canMove()) applyMovement();
		}
  	}

	private void applyGravity(){
		float dt = (frameRate < 50)?(1 / 50f):Time.deltaTime;
		if (edgeState != PlayerEdgeState.HANGING){
			if (velocity.y <= 0)
				velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - upGravity * dt, -terminalVelocity), velocity.z);
			else
				velocity = new Vector3(velocity.x, Mathf.Max(velocity.y - downGravity * dt, -terminalVelocity), velocity.z);
		}else{
			velocity.y = 0;
		}
	}

	private void applyMovement(){
		if (GrabbedCrate()){
			Vector3 drag = Vector3.Dot(velocity, grabAxis) * grabAxis * 0.75f;
			crate.SetVelocity(drag.x, drag.z);
			transform.Translate(drag * Time.deltaTime);
		}else{
			transform.Translate(velocity * Time.deltaTime);
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
		float centerZ	= GetComponent<Collider>().bounds.center.z;

		//array of startpoints
		Vector3[] startPoints = {
			new Vector3(minX, maxY, centerZ),
			new Vector3(maxX, maxY, centerZ),
			new Vector3(minX, minY, centerZ),
			new Vector3(maxX, minY, centerZ),
			new Vector3(centerX, centerY, centerZ)
		};

		//ignore intagible objects
		GameObject[] intangibles = GameObject.FindGameObjectsWithTag("Intangible");
		foreach (GameObject obj in intangibles) {
			obj.layer = 2;
		}

		//check all startpoints
		RaycastHit hitInfo = new RaycastHit();
		for(int i = 0; i < startPoints.Length; i++){
			connected = connected || Physics.Raycast(startPoints[i], Vector3.forward,out hitInfo)
				|| Physics.Raycast(startPoints[i], -1 * Vector3.forward,out hitInfo);
		}

		//set overlap in flip fail indicator
		GetComponentInChildren<FlipFailIndicator>().setOverlappingBlock(hitInfo.collider);

		foreach (GameObject obj in intangibles) {
			obj.layer = 0;
		}

		return connected;
	}


    #region EdgeGrabbing

    private void ReleaseEdge(){
		if(grabbedEdge!=null)
			grabbedEdge.resetStatus();
		grabbedEdge = null;
		edgeState = PlayerEdgeState.FAR_FROM_EDGE;
	}

   //note: this is only called from the Edge.cs
	public void UpdateEdgeState(Edge e, byte edgeState){
		UpdateEdgeState(e,edgeState,-1);
	}

	public void UpdateEdgeState(Edge e, byte edgeState, int animState){
		switch(edgeState){
			case 0:
				if(grabbedEdge != null && e!= null){
					if(grabbedEdge == e){
						this.edgeState = PlayerEdgeState.FAR_FROM_EDGE;
						grabbedEdge =null;
					}
				}

				//adjust animation state
				if(animState!= -1)
					animator.updateEdgeState(animState);

				if(animState == 5)
					climbTimer = CLIMB_TIME;
				break;
			case 1:
				this.edgeState = PlayerEdgeState.CLOSE_TO_EDGE;
				grabbedEdge = e;
				break;
			case 2:
				this.edgeState	 = PlayerEdgeState.HANGING;
				//stop moving
				velocity = Vector3.zero;
				//lock y
				Vector3 pos = transform.position;
				pos.y = e.gameObject.transform.position.y + (e.gameObject.transform.lossyScale.y * .5f) - (colliderHeight * .5f);
				//z-lock to edge
				if(!GameStateManager.is3D())
					pos.z = grabbedEdge.transform.position.z;
				//update transform
				transform.position = pos;
				break;
		}
	}

	public void Grab(Crate crate) {
		this.crate = crate;
		if (crate == null)
			return;
		float xDiff = Mathf.Abs(crate.transform.position.x - transform.position.x);
		float zDiff = Mathf.Abs (crate.transform.position.z - transform.position.z);
		bool closerAlongXAxis = xDiff > zDiff;
		if (!GameStateManager.is3D() || closerAlongXAxis) {
			grabAxis = Vector3.right;
		} else {
			grabAxis = Vector3.forward;
		}
	}

	#endregion EdgeGrabbing

	#region Accessor Methods

	public bool GrabbedCrate() { return crate != null; }

	public Crate GetCrate() { return crate; }

	public bool isPassivelyPushing(){ return passivePush; }

	public Vector3[] getCuboid(){ return cuboid; }

	public void Teleport(Vector3 newPos){
		transform.position = newPos;
		gameObject.GetComponent<BoundObject>().updateBounds();
        gameObject.GetComponent<BoundsVisualizer>().updateBounds();
        DropOrb();
	}

	public Vector3 GetVelocity(){ return velocity; }

	public bool isPaused(){ return _paused; }

	public void setCutsceneMode(bool c){
		if(c == false){
			cutsceneModeDisableTime = Time.time;
		}
		cutsceneMode = c; }

    public bool getCutsceneMode(){
        return cutsceneMode;}

	public bool canMove(){
		return !isDisabled() && !isKicking() && !isClimbing() &&
			!isRiding();
	}

	public bool canControl(){
		return canMove() && !launched && !isInCactusKnockBack();
	}

	public bool isDisabled(){
		bool recentCutsceneModeDisable =
			Time.time - cutsceneModeDisableTime < .1f;
		return isPaused() || cutsceneMode || recentCutsceneModeDisable;
	}

	public float getDimensionAlongAxis(int axis){
		switch(axis){
			case X: return colliderWidth;
			case Y: return colliderHeight;
			case Z: return colliderDepth;
			default:
				throw new System.ArgumentException("Invalid Axis Index");
		}
	}

	public float getColliderWidth(){ return colliderWidth; }

	public float getColliderHeight(){ return colliderHeight; }

	public float getColliderDepth(){ return colliderDepth; }

	public PlayerEdgeState getEdgeState(){ return edgeState; }

	public float getSpeed(){
		return velocity.magnitude;
	}

	//TODO store axis info in grabbed edge
	public int getEdgeOrientation(){ return grabbedEdge.getOrientation(); }

	public Vector3 getGrabAxis(){ return grabAxis; }

	public bool isClimbing(){ return climbTimer > 0; }

	public bool isKicking(){ return kickTimer > 0; }

	public bool isInCactusKnockBack(){ return cactusKnockBackTimer > 0; }

	public bool isRunning(){
		bool movingFast = velocity.magnitude > maxSpeed/2;
		return !isRiding() && isGrounded() && movingFast;
	}

	public bool isWalking(){
		bool movingSlow = 0 < velocity.magnitude && velocity.magnitude <= maxSpeed/2;
		return !isRiding() && isGrounded() && movingSlow;
	}

	public bool isFalling(){
		bool onEdge = edgeState == PlayerEdgeState.HANGING;
		return !isRiding() && velocity.y < -epsilon && !onEdge;
	}

    public bool isGrounded(){
		return isRiding() || grounded;
	}

	public bool isRiding() {
		return ridingPlatform != null;
	}

	public void setRiding(GameObject platform) {
		ridingPlatform = platform;
	}

	public bool isJumping(){
		return !isRiding() && jumping;
	}

	public bool isShimmying(){
		bool moving = velocity.magnitude > epsilon;
		bool onEdge = edgeState == PlayerEdgeState.HANGING;
		return moving && onEdge;
	}

	public bool isLaunched(){ return launched; }

	public bool isHoldingOrb(){ return orb != null; }

	public bool isHoldingOrb( Orb o){ return orb == o; }

	public void grabOrb(Orb newOrb){ orb = newOrb; }

	public Orb getOrb(){
		return orb;
	}

	public int getBoundIndex() {
		return gameObject.GetComponent<BoundObject>().GetBoundIndex();
	}

	#endregion Accessor Methods

	private void OnPauseGame(bool p){
		if(p)
			animator.pauseAnimation();
		else
			animator.resumeAnimation();

		_paused = p;
	}

	public void StartKick(){
	  kickTimer = KICK_TIME;
	  velocity.x = 0;
	  velocity.z = 0;
	}
}

public enum PlayerEdgeState{
	FAR_FROM_EDGE, CLOSE_TO_EDGE, HANGING
}

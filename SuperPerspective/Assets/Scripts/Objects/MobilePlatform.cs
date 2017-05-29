using UnityEngine;
using System.Collections;

public class MobilePlatform : ActiveInteractable {

	public float testYVel = 0f;
	public Vector3 testStartPos;

	public float acceleration = 1.5f;
	public float decelleration = 15f;
	public float maxSpeed = 8f;
	public bool controlled = false;
    public float vol;
    bool respawnFlag = false;
	Vector3 startPos;
	public bool atStart = false;
    public ParticleSystem particle1, particle2;

	float prevY;

	private float colliderHeight, colliderWidth, colliderDepth;

	private const int X = 0;
	private const int Y = 1;
	private const int Z = 2;

	private bool collisionFlagX = false, collisionFlagZ = false;
	private CollisionChecker colCheck;
	private float Margin = 0.05f;

	private Rect rect;

    private AudioSource audio;
    public AudioSource breakSFX;

    ParticleSystem.EmissionModule em1, em2;

    void Start() {
				atStart = true;
        vol = 1f;
				StartSetup();
				colCheck = new CollisionChecker (GetComponent<Collider> (), 3);
				colliderHeight = GetComponent<Collider>().bounds.size.y;
				colliderWidth = GetComponent<Collider>().bounds.size.x;
				colliderDepth = GetComponent<Collider>().bounds.size.z;
				rect = GetComponent<BoundObject>().GetBounds();
				CameraController.instance.TransitionEndingEvent += checkFlipBreak;
				startPos = transform.position;
		    audio = GetComponent<AudioSource>();
		    em1 = particle1.emission;
		    em2 = particle2.emission;
		    //transform.parent = null;

			prevY = transform.position.y;
    }

	void FixedUpdate () {
		base.FixedUpdateLogic();

		if (!collisionFlagX)
			moveOnAxis(X);
		if (!collisionFlagZ)
			moveOnAxis(Z);
		CheckCollisions();
	}

	void Update() {
		//ensure we stay at start if moving vertically
		if(prevY != transform.position.y && atStart){
			transform.localPosition = Vector3.zero;
		}

		if (player.transform.position.y - 0.5f < transform.position.y) {
			range = float.MinValue;
		} else {
			range = GetComponent<Collider>().bounds.size.y + 1f;
		}
        adjustSound();
        toggleParticles();
    }

	void LateUpdate() {
		LateUpdateLogic();
		transform.Translate(velocity * Time.deltaTime);
		if (respawnFlag) {
			Vector3 pos = transform.position;
			if(prevY == pos.y){
				pos = startPos;
				transform.position = pos;
			}else{
				transform.localPosition = Vector3.zero;
			}
			GetComponent<Collider>().enabled = true;
			GetComponentInChildren<Renderer>().enabled = true;
			GetComponent<LevelGeometry>().AdjustPosition(GameStateManager.instance.currentPerspective);
			respawnFlag = false;
			atStart = true;
		}
		collisionFlagX = false;
		collisionFlagZ = false;
	}

	private void moveOnAxis(int axis){
		float newVelocity = velocity[axis];

		float axisInput = (axis == X)?
			InputManager.instance.GetForwardMovement():
				-InputManager.instance.GetSideMovement();

		if (!controlled || PlayerController.instance.isDisabled())
			axisInput = 0;

		if (axisInput != 0){
			newVelocity += acceleration * Mathf.Sign(axisInput);
			newVelocity = Mathf.Clamp(newVelocity,  -maxSpeed * Mathf.Abs(axisInput), maxSpeed * Mathf.Abs(axisInput));
		} else if (velocity[axis] != 0) {
			newVelocity -= decelleration * Mathf.Sign(newVelocity);
			if(Mathf.Sign(newVelocity) != Mathf.Sign(velocity[axis])) newVelocity = 0;
		}

		velocity[axis] = newVelocity;
	}

	public void CheckCollisions() {
		Vector3 trajectory;

		RaycastHit[] hits = colCheck.CheckYCollision (velocity, Margin);

		// If any rays connected move the player and set grounded to true since we're now on the ground

		float close;

		if (velocity.x != 0) {
			hits = colCheck.CheckXCollision (velocity, Margin);
			close = -1;
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hitInfo = hits[i];
				if (hitInfo.collider != null)
				{
					if (close == -1 || close > hitInfo.distance) {
						close = hitInfo.distance;
						//transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (hitInfo.distance - colliderWidth / 2));
						trajectory = velocity.x * Vector3.right;
					}
				}
			}
			if (close != -1) {
				//transform.Translate(Vector3.right * Mathf.Sign(velocity.x) * (close - colliderWidth / 2));
				velocity = new Vector3(0f, velocity.y, velocity.z);
			}
		}

		if (velocity.z != 0) {
			hits = colCheck.CheckZCollision (velocity, Margin);
			close = -1;
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hitInfo = hits[i];
				if (hitInfo.collider != null)
				{
					if (close == -1 || close > hitInfo.distance) {
						close = hitInfo.distance;
						//transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (hitInfo.distance - colliderDepth / 2));
						trajectory = velocity.z * Vector3.forward;
					}
				}
			}
			if (close != -1) {
				//transform.Translate(Vector3.forward * Mathf.Sign(velocity.z) * (close - colliderDepth / 2));
				velocity = new Vector3(velocity.x, velocity.y, 0f);
			}
		}

		float vertVel = transform.position.y - prevY;
		testYVel = vertVel;
		if(vertVel != 0){
			Vector3 vel = new Vector3(0f,vertVel,0f);
			hits = colCheck.CheckYCollision (vel, Margin);
			close = -1;
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hitInfo = hits[i];
				if (hitInfo.collider != null)
				{
					if (close == -1 || close > hitInfo.distance) {
						close = hitInfo.distance;
					}
				}
			}
			if (close != -1) {
				//print("break");
				doBreak();
			}
		}
		prevY = transform.position.y;
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


		RaycastHit hitInfo = new RaycastHit();
		float rad = (GetComponent<Collider>().bounds.max.z - GetComponent<Collider>().bounds.max.z) * 0.5f;
		connected =
			Physics.CapsuleCast(
					new Vector3(minX + rad, centerY, centerZ),
					new Vector3(maxX - rad, centerY, centerZ),
					rad, Vector3.forward, out hitInfo) ||
			Physics.CapsuleCast(
					new Vector3(minX + rad, centerY, centerZ),
					new Vector3(maxX - rad, centerY, centerZ),
					rad, Vector3.back, out hitInfo);

		//set overlap in flip fail indicator
		GetComponentInChildren<FlipFailIndicator>().setOverlappingBlock(hitInfo.collider);

		return connected;
	}

	void checkFlipBreak() {
		if(GameStateManager.is2D() && !GameStateManager.isFailedShift() && Check2DIntersect()) {
			doBreak();
		}
	}

	private void doBreak(){
		if (controlled){
				PlayerController.instance.setRiding(null);
				controlled = false;
				player.transform.parent = null;
			}
			respawnFlag = true;
      breakSFX.Play();
	}

	public override void Triggered() {
		if (!controlled && player.transform.position.y > transform.position.y) {
			PlayerController.instance.setRiding(this.gameObject);
			controlled = true;
			atStart = false;
			player.transform.Translate(transform.position.x - player.transform.position.x, 0, transform.position.z - player.transform.position.z);
			player.transform.parent = transform;
			player.GetComponent<PlayerController>().setVelocity(Vector3.zero);
		} else {
			PlayerController.instance.setRiding(null);
			controlled = false;
			player.transform.parent = null;
		}
	}

	public override float GetDistance() {
		Vector3 pPos = player.transform.position;
		Bounds bounds = GetComponent<Collider>().bounds;
		if (pPos.x >= bounds.min.x && pPos.x <= bounds.max.x && pPos.z >= bounds.min.z && pPos.z <= bounds.max.z) {
			return pPos.y - transform.position.y;
		}
		return float.MaxValue;
	}

	protected override bool isPlayerFacingObject() {
		return true;
	}
	protected override bool isPlayerFacingObject2D() {
		return true;
	}

	protected override bool IsInYRange(){
		float playerY = PlayerController.instance.transform.position.y;
		float myY = transform.position.y;
		float deltaY = playerY - myY;
		return 1.3f < deltaY && deltaY < 3f;
	}

	public void setCollisionFlag(int axis) {
		if (axis == 0)
			collisionFlagX = true;
		else
			collisionFlagZ = true;
	}

	/*public float GetStartY() {
		//this.transform.parent.gameObject.transform.position.y;
		return startPos.y;
	}*/

    private void adjustSound()
    {
        if (controlled)
        {
            if (audio.volume < vol)
            {
                audio.volume += Time.deltaTime*2f;
                if (audio.volume > vol)
                    audio.volume = vol;
            }
        }
        else
        {
            if (audio.volume > 0)
            {
                audio.volume -= Time.deltaTime *2f;
                if (audio.volume < 0)
                    audio.volume = 0;
            }
        }
    }

    private void toggleParticles()
    {
        if (controlled)
        {
            em1.enabled = true;
            em2.enabled = true;
        }
        else
        {
            em1.enabled = false;
            em2.enabled = false;
        }
    }
}

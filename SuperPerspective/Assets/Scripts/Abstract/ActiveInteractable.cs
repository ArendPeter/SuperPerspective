﻿using UnityEngine;
using System.Collections;

//can be activated by player
public class ActiveInteractable : PhysicalObject
{

    //For glowiness
    [SerializeField]
    private Renderer glowRenderer;

    public bool notificationSuppressed = false;

    //suppress warnings
#pragma warning disable 414

    bool ignoreYDistance = true;

    //player
    protected GameObject player;

    //main ActiveInteractable
    static ActiveInteractable main;

    //keeps track of notification marker
    static NotificationController notiMarker;

    //whether notification is shown
    static bool notiShown = false;
    static float notiDist = -1;
    static ActiveInteractable selected = null;
    static int frameCount = 0;

    //used to create a fixedlateupdate effect
    bool fixedCalled = false;

    //distance for inRange
    protected float range = 1f;

    //how much error there can be in the angle for it to be valid
    float angleBuffer = 80;

    public bool invisible = false;

    public bool isInteractable = true;

    private int boundIndex;

    void Start()
    {
        StartSetup();
    }

    void FixedUpdate()
    {
        if (isInteractable)
            FixedUpdateLogic();
    }

    void LateUpdate()
    {
        if (isInteractable)
            LateUpdateLogic();
    }

    void InteractPressed()
    {
        if (!GameStateManager.IsGamePaused() && !PlayerController.instance.isDisabled() && selected == this)
            Triggered();
    }

    public virtual void Triggered() { }

    protected void StartSetup()
    {
        base.Init();
        //find player
        player = PlayerController.instance.gameObject;
        //become main if no one else has become it yet
        if (main == null) main = this;
        //perform static actions
        if (main == this)
        {
            //find notification marker
            notiMarker = player.transform.Find("Notification").GetComponent<NotificationController>();
            //disable it so it will be invisible
            notiMarker.updateVisible(notiShown);
        }
        //register interactpressed to the InputManager
        InputManager.instance.InteractPressedEvent += InteractPressed;

        //Get your material
        glowRenderer = GetComponentInChildren<Renderer>();

        Vector3 pos = transform.position;
        boundIndex = IslandControl.instance.getBound(pos.x, pos.y, pos.z, !GameStateManager.is3D());
    }

    protected void FixedUpdateLogic()
    {
        int playerBoundIndex = player.GetComponent<PlayerController>().getBoundIndex();

        bool canTrigger = false, notificationCanBeShown = false;
        float dist = 0f;
        if (playerBoundIndex == boundIndex)
        {
            dist = GetDistance();

            bool inRange = dist < range;

            bool playerFacing = GameStateManager.is3D() ? isPlayerFacingObject() : isPlayerFacingObject2D();

            bool inYRange = IsInYRange();

            canTrigger =
                inRange && playerFacing && inYRange && IsEnabled();

            //checks for competing notifications (I think)
            notificationCanBeShown = (!notiShown || dist < notiDist);
        }

        //update notiShown
        if (player.GetComponent<PlayerController>().canInteract)
        {
            notiMarker.updateVisible(true);
        }
        else if (canTrigger && notificationCanBeShown && !DevConsoleController.instance.isConsoleActive())
        {
            selected = this;
            notiShown = true;
            if (!notificationSuppressed)
            {
                notiMarker.updateVisible(true);
            }
            notiDist = dist;
        }

        if (glowRenderer != null)
        {
            if (canTrigger)
            {
                glowRenderer.material.SetFloat("_RimPower", .5f);
            }
            else
            {
                glowRenderer.material.SetFloat("_RimPower", 8.0f);
            }
        }

        fixedCalled = true;
    }

    protected virtual bool IsEnabled()
    {
        bool isVisible = false;
        if (!invisible)
        {
            isVisible = GetComponentInChildren<Renderer>().enabled;
        }
        else
        {
            isVisible = false;
        }

        bool unlockable = ((this.gameObject.GetComponent<LockedDoor>() == null || Key.GetKeysHeld() > 0));

        return isVisible && unlockable;
    }

    public virtual float GetDistance()
    {
        float colMinX = GetComponent<Collider>().bounds.min.x;
        float colMaxX = GetComponent<Collider>().bounds.max.x;
        float colMinZ = GetComponent<Collider>().bounds.min.z;
        float colMaxZ = GetComponent<Collider>().bounds.max.z;
        switch (GetQuadrant())
        {
            case Quadrant.xPlus:
                return player.transform.position.x - colMaxX;
            case Quadrant.xMinus:
                return colMinX - player.transform.position.x;
            case Quadrant.zPlus:
                return player.transform.position.z - colMaxZ;
            case Quadrant.zMinus:
                return colMinZ - player.transform.position.z;
            default:
                return float.MaxValue;
        }
    }

    protected Quadrant GetQuadrant()
    {
        float colliderWidth = GetComponent<Collider>().bounds.size.x;
        float colliderDepth = GetComponent<Collider>().bounds.size.z;
        if (Mathf.Abs(player.transform.position.x - transform.position.x) > colliderWidth / 2 || GameStateManager.is2D())
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) <= colliderDepth / 2 || GameStateManager.is2D())
            {
                if (player.transform.position.x - transform.position.x > 0)
                    return Quadrant.xPlus;
                else
                    return Quadrant.xMinus;
            }
        }
        else if (Mathf.Abs(player.transform.position.z - transform.position.z) > colliderDepth / 2)
        {
            if (Mathf.Abs(player.transform.position.x - transform.position.x) <= colliderWidth / 2)
            {
                if (player.transform.position.z - transform.position.z > 0)
                    return Quadrant.zPlus;
                else
                    return Quadrant.zMinus;
            }
        }
        return Quadrant.none;
    }

    protected virtual bool isPlayerFacingObject()
    {
        float playerOrientation = PlayerAnimController.instance.getOrientation();
        playerOrientation = (playerOrientation + 360) % 360;

        //calculate angle between interactable and player
        float playerAngle = Vector2.Angle(new Vector2(transform.position.x - player.transform.position.x,
                                                      transform.position.z - player.transform.position.z), Vector2.up);

        bool inFrontOfPlayer = transform.position.x < player.transform.position.x;
        if (inFrontOfPlayer)
        {
            playerAngle = 360 - playerAngle;
        }

        //calculate difference and modify so that it's in the correct range
        float angleDiff = Mathf.Abs(playerOrientation - playerAngle);
        angleDiff += 360;
        angleDiff %= 360;
        if (angleDiff > 180)
            angleDiff = 360 - angleDiff;
        //determine whether player is facing interactable
        return angleDiff < angleBuffer;
    }

    protected virtual bool isPlayerFacingObject2D()
    {
        float playerOrientation = PlayerAnimController.instance.getOrientation();
        playerOrientation = (playerOrientation + 360) % 360;
        if (transform.position.x > player.transform.position.x)
            return playerOrientation < 180;
        return playerOrientation > 180;
    }

    protected virtual bool IsInYRange()
    {
        return yRangeOverlapsWithPlayer();
    }

    private bool yRangeOverlapsWithPlayer()
    {
        float colliderTop, colliderBot;
        if (GetComponent<Collider>() == null)
        {
            colliderTop = transform.position.y;
            colliderBot = transform.position.y;
        }
        else
        {
            colliderBot = GetComponent<Collider>().bounds.min.y + 0.05f;
            colliderTop = GetComponent<Collider>().bounds.max.y - 0.05f;
        }
        float playerBot = player.GetComponent<Collider>().bounds.min.y;
        float playerTop = player.GetComponent<Collider>().bounds.max.y;

        bool rangesOverlap = (colliderTop > playerBot && colliderBot < playerTop);

        return rangesOverlap || !ignoreYDistance;
    }

    protected void LateUpdateLogic()
    {
        //perform static actions
        if (main == this && fixedCalled)
        {
            frameCount++;
            //make notification invisible if no interactables could trigger it
            if (!notiShown && !player.GetComponent<PlayerController>().canInteract)
            {
                notiMarker.updateVisible(false);
                selected = null;
            }
            //prepare for next frame
            notiShown = false;

            fixedCalled = false;
        }
    }

    public void setNotMarkerVisibility(bool set)
    {
        notiMarker.updateVisible(set);
    }

    public enum Quadrant
    {
        zPlus, zMinus, xPlus, xMinus, none
    }
}

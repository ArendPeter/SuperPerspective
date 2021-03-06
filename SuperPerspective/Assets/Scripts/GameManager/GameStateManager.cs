﻿using UnityEngine;
using System.Collections;

/// <summary>
///     This class is in charge of controlling the game state transitions.
///     It receives input from InputManager, and tells the camera, player, and any other necessary objects of behavior changes.
///     This keeps all transitions logic in one location and allows the player, camera, and menu scripts to have simple behavior and remain modular.
/// </summary>
public class GameStateManager : MonoBehaviour
{
	public static GameStateManager instance;

	//suppress warnings
	#pragma warning disable 414, 649, 472

	#region Properties & Variables

	// State variables
	public ViewType testCurrentState;

	public ViewType currentState { get; private set; }
	public ViewType previousState { get; private set; }
	public ViewType targetState { get; private set; }
	//TODO combine PerspectiveType and Matrix4x4 to have same purpose
	public PerspectiveType currentPerspective{ get; private set; }

	// Flip failure timer variables
	private bool failedShift = false;

	// view mounts & settings
	private const int NUM_VIEW_TYPES = 10;
	private Transform[] view_mounts = new Transform[NUM_VIEW_TYPES];
	private PerspectiveType[] view_perspectives = new PerspectiveType[NUM_VIEW_TYPES];
	private bool[] view_pause = new bool[NUM_VIEW_TYPES];

	// Events to notify listeners of state changes
	public event System.Action<bool> GamePausedEvent;
	public event System.Action<PerspectiveType> PerspectiveShiftEvent;//called at end  of shift
	//either PerspectiveShiftSuccessEvent or PerspectiveShiftFailEvent will be call at the beginning of shifts
	public event System.Action PerspectiveShiftSuccessEvent;
	public event System.Action PerspectiveShiftFailEvent;
    public event System.Action PausePressedUpdateUIEvent;

    public Pause_UI pause_UI;

    bool specialCase = false;

	#endregion Properties & Variables

	#region Initialization

	public void Awake () {
		//singleton
		if (instance == null)
			instance = this;
		else
			Destroy (this);
	}

	void Start(){
		InitViewPerspectives();
		InitViewMounts();
		InitViewPauseStates();

		RegisterEventHandlers();
        InitPausePressedUpdateUIEvent();

        GoToStartState();
	}

	void InitViewPerspectives(){
		view_perspectives[(int)ViewType.STANDARD_3D] =   PerspectiveType.p3D;
		view_perspectives[(int)ViewType.STANDARD_2D] =   PerspectiveType.p2D;
		view_perspectives[(int)ViewType.PAUSE_MENU] =    PerspectiveType.p3D;
		view_perspectives[(int)ViewType.WAYSTONE_MENU] = PerspectiveType.p3D;
		view_perspectives[(int)ViewType.MENU] =          PerspectiveType.p2D;
		view_perspectives[(int)ViewType.LEAN_LEFT] =     PerspectiveType.p3D;
		view_perspectives[(int)ViewType.LEAN_RIGHT] =	 PerspectiveType.p3D;
		view_perspectives[(int)ViewType.BACKWARD] =		 PerspectiveType.p3D;
		view_perspectives[(int)ViewType.DYNAMIC] =		 PerspectiveType.p3D;
	}

	void InitViewMounts(){
		// Gather mount gameobjects
		GameObject[] mounts = new GameObject[NUM_VIEW_TYPES];
		mounts[(int)ViewType.STANDARD_3D] = GameObject.Find("3DCameraMount");
		mounts[(int)ViewType.STANDARD_2D] = GameObject.Find("2DCameraMount");
		mounts[(int)ViewType.MENU] = GameObject.Find("MenuMount");
		mounts[(int)ViewType.WAYSTONE_MENU] = GameObject.Find("PauseMount");
		mounts[(int)ViewType.LEAN_LEFT] = GameObject.Find("LeanLeftCameraMount");
		mounts[(int)ViewType.LEAN_RIGHT] = GameObject.Find("LeanRightCameraMount");
		mounts[(int)ViewType.BACKWARD] = GameObject.Find("BackwardCameraMount");

		//find transforms
		for(int i = 0; i < mounts.Length; i++){
			if(mounts[i] != null)
				view_mounts[i] = mounts[i].transform;
		}
	}

	void InitViewPauseStates(){
		view_pause[(int)ViewType.STANDARD_3D] =   false;
		view_pause[(int)ViewType.STANDARD_2D] =   false;
		view_pause[(int)ViewType.PAUSE_MENU] =    true;
		view_pause[(int)ViewType.WAYSTONE_MENU] = true;
		view_pause[(int)ViewType.MENU] =          true;
		view_pause[(int)ViewType.LEAN_LEFT] =     false;
		view_pause[(int)ViewType.LEAN_RIGHT] =	  false;
		view_pause[(int)ViewType.BACKWARD] =	  false;
		view_pause[(int)ViewType.DYNAMIC] =		  false;
	}

	void RegisterEventHandlers(){
		InputManager.instance.ShiftPressedEvent += HandleShiftPressed;
		InputManager.instance.PausePressedEvent += HandlePausePressed;
		InputManager.instance.LeanLeftPressedEvent += HandleLeanLeftPressed;
		InputManager.instance.LeanRightPressedEvent += HandleLeanRightPressed;
		InputManager.instance.LeanLeftReleasedEvent += HandleLeanLeftReleased;
		InputManager.instance.LeanRightReleasedEvent += HandleLeanRightReleased;
		InputManager.instance.BackwardMovementEvent += HandleBackwardMovement;
		InputManager.instance.ForwardMovementEvent += HandleForwardMovement;
		InputManager.instance.InteractPressedEvent += HandleInteractPressed;

		CameraController.instance.TransitionCompleteEvent += HandleTransitionComplete;
	}

	void GoToStartState(){
		if(view_mounts[(int)ViewType.MENU] == null)
			StartGame();
		else
			EnterState(ViewType.MENU);
        pause_UI = FindObjectOfType<Pause_UI>();
	}

	#endregion Monobehavior Implementation

	#region Event Handlers
	public void FixedUpdate(){
			checkForBlinkEnd();
	}

    private void InitPausePressedUpdateUIEvent()
    {
        //Updates PausePressedUpdateUIEvent with references to the CrystalCountText so that it can be used to update crystal counts upon pausing
        CrystalCountText[] crystalCount = Resources.FindObjectsOfTypeAll(typeof(CrystalCountText)) as CrystalCountText[];
        foreach (CrystalCountText count in crystalCount)
        {
            PausePressedUpdateUIEvent += count.UpdateValues;
        }
    }

    private void checkForBlinkEnd(){
		bool blinking = false;
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("FlipFailIndicator")){
			FlipFailIndicator flipper = obj.GetComponent<FlipFailIndicator>();
			blinking = blinking || flipper.isBlinking();
		}
		if(failedShift && !isTransitioning() &&
				!blinking && !Input.GetButton("PerspectiveShift")){
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("FlipFailIndicator")){
				obj.GetComponent<FlipFailIndicator>().disableVisible();
			}
			EnterState(ViewType.STANDARD_3D);
			failedShift = false;
		}
	}

	public void EnterWaystoneState() {
		EnterState(ViewType.WAYSTONE_MENU);
	}

	public void ExitWaystoneState() {
		EnterState(previousState);
	}

	//must be public so it can be called from continue button
	public void HandlePausePressed(){
		if(currentState == ViewType.WAYSTONE_MENU || targetState == ViewType.WAYSTONE_MENU || (PlayerController.instance != null && !PlayerController.instance.isPaused() &&PlayerController.instance.isDisabled()) ||
            (BigCrystalGet.instance != null && BigCrystalGet.instance.uiActive))
			return;
		switch(currentState){
			case ViewType.MENU:
				if(previousState != null)
					EnterState(previousState);
                pause_UI.isPaused = false;
            break;
			case ViewType.PAUSE_MENU:
				if(previousState != ViewType.PAUSE_MENU){
					EnterState(previousState);
	            pause_UI.isPaused = false;
				}
            break;
			default:
				EnterState(ViewType.PAUSE_MENU);
                if (PausePressedUpdateUIEvent != null)
                {
                    PausePressedUpdateUIEvent();
                }
                pause_UI.isPaused = true;
            break;
		}
	}

    public void ShiftSpecialCase()
    {
        specialCase = true;
        HandleShiftPressed();
    }

	private void HandleShiftPressed(){
		if (specialCase || (!failedShift && !IsPauseState(targetState) && !PlayerController.instance.GrabbedCrate()
             && PlayerPrefs.GetString("IntroCutsceneFinished") == "true" && PlayerController.instance.getCutsceneMode() == false))
        {
			ViewType newState = (view_perspectives[(int)currentState] == PerspectiveType.p3D) ?
				ViewType.STANDARD_2D : ViewType.STANDARD_3D;


			bool playerIntersects = PlayerController.instance.Check2DIntersect();
			bool platformIntersects = false;
			foreach(GameObject ind in GameObject.FindGameObjectsWithTag("FlipFailIndicator")){
				MobilePlatform plat = ind.transform.parent.GetComponent<MobilePlatform>();
				if(plat == null || !plat.controlled) continue;
				platformIntersects = platformIntersects || plat.Check2DIntersect();
			}
			if(playerIntersects || platformIntersects){
				RaisePerspectiveShiftFailEvent();
				failedShift = true;
			}else{
				RaisePerspectiveShiftSuccessEvent();
			}

			EnterState(newState);
		}
        specialCase = false;
    }

	private void HandleMenuEnterPressed(){
		if (currentState == ViewType.MENU){
			EnterState(previousState);
		}else{
			EnterState(ViewType.MENU);
		}
	}

	private void HandleTransitionComplete(){
		currentState = targetState;
		testCurrentState = currentState;

		if(currentState == ViewType.STANDARD_2D ){
			CameraController.instance.setCameraOffset(new Vector3(0,0,-100));
		}
		if(failedShift){
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("FlipFailIndicator")){
				FlipFailIndicator flipper = obj.GetComponent<FlipFailIndicator>();
				flipper.blink();
			}
		}

		RaisePerspectiveShiftEvent();

		RaisePauseEvent();
	}

	private void HandleLeanLeftPressed(){
		if(!IsPauseState(currentState) && currentPerspective == PerspectiveType.p3D && PlayerController.instance != null && !PlayerController.instance.isDisabled())
			EnterState(ViewType.LEAN_LEFT);
	}

	private void HandleLeanRightPressed(){
		if(!IsPauseState(currentState) && currentPerspective == PerspectiveType.p3D && PlayerController.instance != null && !PlayerController.instance.isDisabled())
			EnterState(ViewType.LEAN_RIGHT);
	}

	private void HandleLeanLeftReleased(){
		if(targetState == ViewType.LEAN_LEFT && currentPerspective == PerspectiveType.p3D){
			EnterState(ViewType.STANDARD_3D);
		}
	}

	private void HandleLeanRightReleased(){
		if(targetState == ViewType.LEAN_RIGHT && currentPerspective == PerspectiveType.p3D)
			EnterState(ViewType.STANDARD_3D);
	}

	private void HandleBackwardMovement(){
		if(InputManager.instance.GetForwardMovement() == -1 && currentState == ViewType.STANDARD_3D && !isTransitioning()){
			EnterState(ViewType.BACKWARD);
		}
	}

	private void HandleForwardMovement(){
		if(InputManager.instance.GetForwardMovement() == 1 && currentState == ViewType.BACKWARD){
			EnterState(ViewType.STANDARD_3D);
		}
	}

	private void HandleInteractPressed(){
		if(onDynamicState()){
			ExitDynamicState();
		}
	}

	#endregion Event Handlers

	#region State Change Functions

	private void EnterState(ViewType newState){
		bool targetOnNewState = (newState == targetState);
		if(targetOnNewState) return;

		UpdatePauseMount();
		CheckForPauseMenu(newState);

		previousState = targetState;
		targetState = newState;
		RaisePauseEvent();
		currentPerspective = view_perspectives[(int)newState];
		if(previousState == ViewType.STANDARD_2D ){
			CameraController.instance.setCameraOffset(new Vector3(0,0,0));
		}
		CameraController.instance.SetMount(view_mounts[(int)newState],currentPerspective);
	}

	private void CheckForPauseMenu(ViewType targetState){
		if(targetState != ViewType.PAUSE_MENU && currentState != ViewType.PAUSE_MENU)
			return;

		bool goingToPauseMenu = (targetState == ViewType.PAUSE_MENU);

		PauseMenu.instance.UpdateMenuVisible(goingToPauseMenu);
	}

	private void UpdatePauseMount(){
		Transform newMount = IslandControl.instance.findCurrentPauseMount();
		if(newMount == null){
			view_mounts[(int)ViewType.PAUSE_MENU] = view_mounts[(int)ViewType.STANDARD_3D];
			view_mounts[(int)ViewType.WAYSTONE_MENU] = view_mounts[(int)ViewType.STANDARD_3D];
		}else{
			view_mounts[(int)ViewType.PAUSE_MENU] = newMount;
			view_mounts[(int)ViewType.WAYSTONE_MENU] = newMount;
		}
	}

   #endregion State Change Functions

   #region Event Raising Functions

	// Alert listeners that the game is being paused or unpaused
	private void RaisePauseEvent(){
		if (GamePausedEvent != null)
			GamePausedEvent(IsGamePaused());
	}

	private void RaisePerspectiveShiftEvent(){
		if (PerspectiveShiftEvent != null)
			PerspectiveShiftEvent(currentPerspective);
	}

	private void RaisePerspectiveShiftSuccessEvent(){
		if (PerspectiveShiftSuccessEvent != null)
			PerspectiveShiftSuccessEvent();
	}

	private void RaisePerspectiveShiftFailEvent(){
		if (PerspectiveShiftFailEvent != null)
			PerspectiveShiftFailEvent();
	}

	#endregion Event Raising Functions

	#region Public Interface

	// Called by main menu to begin gameplay
	public void StartGame(){
		EnterState(ViewType.STANDARD_2D);
	}

	public void Reset(){
		InitViewPerspectives();
		InitViewMounts();
		InitViewPauseStates();

		//determine wheather or not to start on menu
		if(view_mounts[(int)ViewType.MENU] == null){
			StartGame();
		}else{
			EnterState(ViewType.MENU);
		}

		// Register event handlers to InputManagers
		InputManager.instance.ShiftPressedEvent += HandleShiftPressed;
		InputManager.instance.PausePressedEvent += HandlePausePressed;
		InputManager.instance.LeanLeftPressedEvent += HandleLeanLeftPressed;
		InputManager.instance.LeanRightPressedEvent += HandleLeanRightPressed;
		InputManager.instance.LeanLeftReleasedEvent += HandleLeanLeftReleased;
		InputManager.instance.LeanRightReleasedEvent += HandleLeanRightReleased;
		InputManager.instance.BackwardMovementEvent += HandleBackwardMovement;
		InputManager.instance.ForwardMovementEvent += HandleForwardMovement;

		// Register to switch state to proper gameplay when shift is complete
		CameraController.instance.TransitionCompleteEvent += HandleTransitionComplete;
	}

	public void EnterDynamicState(Transform t){
		Camera cam = t.gameObject.GetComponent<Camera>();
		if(cam == null)
			throw new System.ArgumentException(
				"Dynamic Camera scripts must only be attached "+
				"to game objects which also have Camera components.");
		bool dyIs2D = cam.orthographic;
		bool curIs2D = (currentPerspective == PerspectiveType.p2D);

		cam.transparencySortMode = TransparencySortMode.Orthographic;//

		if(dyIs2D != curIs2D)
			return;

		view_perspectives[(int)ViewType.DYNAMIC] =
			cam.orthographic? PerspectiveType.p2D : PerspectiveType.p3D;
		view_mounts[(int)ViewType.DYNAMIC] = t;
		EnterState(ViewType.DYNAMIC);
	}

	public void ExitDynamicState(){
		if(targetState != ViewType.DYNAMIC)
			return;
		if(currentPerspective == PerspectiveType.p3D)
			EnterState(ViewType.STANDARD_3D);
		else
			EnterState(ViewType.STANDARD_2D);
	}

	public bool changingPerspective() {
		return view_perspectives[(int)targetState] != view_perspectives[(int)currentState];
	}

	public static bool is3D(){
		return GameStateManager.instance.currentPerspective == PerspectiveType.p3D;
	}

	public static bool is2D(){
		return GameStateManager.instance.currentPerspective == PerspectiveType.p2D;
	}

	public static bool IsGamePaused(){
		return GameStateManager.instance.IsPauseState(GameStateManager.instance.targetState) ||
			GameStateManager.instance.failedShift;
	}

	public static bool onDynamicState(){
		return instance.currentState == ViewType.DYNAMIC;
	}

	public static bool targetingDynamicState(){
		return instance.targetState == ViewType.DYNAMIC;
	}

	public static bool isTransitioning(){
		return instance.targetState != instance.currentState;
	}

	public static bool isFailedShift(){
		return instance.failedShift;
	}

	#endregion Public Interface

	#region Helper Functions
	private bool IsPauseState(ViewType targetState){
		return view_pause[(int)targetState] || currentPerspective != view_perspectives[(int)targetState];
	}
   #endregion

}

/// <summary>
///     Enumeration to dictate which perspective the current state is in.
///     Objects can reference this and it is passed in shioft events so they know which behavior mode to execute.
/// </summary>
public enum PerspectiveType{
    p3D, p2D
}

public enum ViewType{
	NULL_VIEW, STANDARD_3D, STANDARD_2D, MENU, PAUSE_MENU, WAYSTONE_MENU, LEAN_LEFT, LEAN_RIGHT, BACKWARD, DYNAMIC
}

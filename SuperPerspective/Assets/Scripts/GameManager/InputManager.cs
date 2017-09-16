using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
///     Sends notifications on user input and allows polling for current input state of buttons and axes.
///     Also notifies listeners when the game's perspective changes.
/// </summary>
public class InputManager : MonoBehaviour
{

    public static InputManager instance;

    //suppress warnings
#pragma warning disable 414

    #region Properties & Variables

    // Button press events
    public event System.Action JumpPressedEvent;         // Jump
    public event System.Action InteractPressedEvent;     // Interaction
    public event System.Action GrabPressedEvent;         // Grab
    public event System.Action ShiftPressedEvent;
    public event System.Action PausePressedEvent;
    public event System.Action LeanLeftPressedEvent;
    public event System.Action LeanRightPressedEvent;
    public event System.Action LeanLeftReleasedEvent;
    public event System.Action LeanRightReleasedEvent;
    public event System.Action BackwardMovementEvent;
    public event System.Action ForwardMovementEvent;
    public event System.Action DevConsoleEvent;
    public event System.Action MenuUpEvent;
    public event System.Action MenuDownEvent;
    public event System.Action MenuRightEvent;
    public event System.Action MenuLeftEvent;
    public event System.Action HelpEvent;

    // Game's pause state
    private bool continuePressed = false;//used as an alternate way to unpause

    // Perspective shift properties
    // TODO: Move this funcionality to the camera script
    private const float FAIL_TIME = 0.5f, MENU_PAUSE = 0.15f, MENU_PAUSE_INIT = 0.5f;
    private float flipTimer = 0, menuTimer = 0;
    private bool flipFailed = false;

    private float previousForwardMovement = 0, menuMove = 0, prevMenuMove = 0;
    private string menuAxis = "";

    string leanDir;

    #endregion Properties & Variables

    #region Monobehavior Implementation

    void Awake()
    {
        //singleton
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // listens to player input and raises events for listeners.
    void Update()
    {
        if (GetButtonUpAbsolute("DevConsoleToggle"))
            RaiseEvent(DevConsoleEvent);

        if (GetButtonDownAbsolute("Interaction"))
            RaiseEvent(InteractPressedEvent);

        if (GetButtonDownAbsolute("Pause") || continuePressed)
        {
            RaiseEvent(PausePressedEvent);
            continuePressed = false;
        }
        if (GetButtonDownAbsolute("Jump"))
            RaiseEvent(JumpPressedEvent);

        if (GetButtonDownAbsolute("Grab"))
            RaiseEvent(GrabPressedEvent);

		if ((GetButtonDownAbsolute("PerspectiveShift") || Input.GetKeyDown(KeyCode.RightShift)) && GameStateManager.instance != null && !GameStateManager.instance.changingPerspective())
            RaiseEvent(ShiftPressedEvent);

        if (GetButtonDownAbsolute("LeanLeft"))
            RaiseEvent(LeanLeftPressedEvent);

        if (GetButtonDownAbsolute("LeanRight"))
            RaiseEvent(LeanRightPressedEvent);

        if (GetButtonUpAbsolute("LeanLeft"))
            RaiseEvent(LeanLeftReleasedEvent);

        if (GetButtonUpAbsolute("LeanRight"))
            RaiseEvent(LeanRightReleasedEvent);

        if (GetButtonDownAbsolute("Help"))
        {
            RaiseEvent(HelpEvent);
        }

        if (previousForwardMovement != -1 && GetForwardMovement() == -1)
            RaiseEvent(BackwardMovementEvent);

        if (previousForwardMovement != 1 && GetForwardMovement() == 1)
            RaiseEvent(ForwardMovementEvent);

		float moveThreshold = 0.9f, resetThreshold = 0.25f;
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
			menuMove = 1f;
			menuAxis = "Horizontal";
		} else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
			menuMove = -1f;
			menuAxis = "Horizontal";
		} else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
			menuMove = 1f;
			menuAxis = "Vertical";
		} else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
			menuMove = -1f;
			menuAxis = "Vertical";
		}
		if (Input.GetAxis("Menu") != 0) {
			menuMove = Mathf.Sign(Input.GetAxis("Menu"));
			menuAxis = "Vertical";
		} else if (Input.GetAxis("MenuSlider") != 0) {
			menuMove = Mathf.Sign(Input.GetAxis("MenuSlider"));
			menuAxis = "Horizontal";
		}
		if (menuAxis != "" && Mathf.Abs(menuMove) <= resetThreshold) {
			menuTimer = 0;
		}
        if (menuTimer <= 0)
        {
			if (menuMove >= moveThreshold)
            {
                if (menuAxis == "Vertical")
                    RaiseEvent(MenuUpEvent);
                else
                    RaiseEvent(MenuRightEvent);
				if (prevMenuMove < moveThreshold)
                    menuTimer = MENU_PAUSE_INIT;
                else
                    menuTimer = MENU_PAUSE;
            }
			if (menuMove <= -moveThreshold)
            {
                if (menuAxis == "Vertical")
                    RaiseEvent(MenuDownEvent);
                else
                    RaiseEvent(MenuLeftEvent);
				if (prevMenuMove > -moveThreshold)
                    menuTimer = MENU_PAUSE_INIT;
                else
                    menuTimer = MENU_PAUSE;
            }
            prevMenuMove = menuMove;
        }
        else
        {
            menuTimer -= Time.deltaTime;
        }

        previousForwardMovement = GetForwardMovement();
        menuMove = GetMenuMovement();
        GetTiltAxis();
    }

    #endregion MonobehaviorImplementation

    #region Private Functions

    private bool GetButtonAbsolute(string button)
    {
        if (SystemInfo.operatingSystem.StartsWith("Mac"))
        {
            return Input.GetButton("OSX" + button);
        }
        return Input.GetButton(button);
    }


    private bool GetButtonDownAbsolute(string button)
    {
        if (SystemInfo.operatingSystem.StartsWith("Mac"))
        {
            return Input.GetButtonDown("OSX" + button);
        }
        return Input.GetButtonDown(button);
    }

    private bool GetButtonUpAbsolute(string button)
    {
        if (SystemInfo.operatingSystem.StartsWith("Mac"))
        {
            return Input.GetButtonUp("OSX" + button);
        }
        return Input.GetButtonUp(button);
    }

    #endregion

    #region Public Interface

    // Returns the player's movement on the horizontal axis in 2D and the vertical axis in 3D
    public float GetForwardMovement()
    {
        if (SceneManager.GetActiveScene().name == "CreditsScene")
        {
            //Workaround for my credits scene -Nick
            return Input.GetAxis("Horizontal");
        }
        if (GameStateManager.instance != null)
        {
            if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
                return Input.GetAxis("Vertical");
            else
                return Input.GetAxis("Horizontal");
        }
        return 0;
    }

    public void GetTiltAxis()
    {
        if (GameStateManager.instance != null && GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
        {
            if (SystemInfo.operatingSystem.StartsWith("Mac"))
            {
                if (Input.GetAxis("RightTiltControllerMac") > 0.1)
                {
                    RaiseEvent(LeanLeftPressedEvent);
                    leanDir = "left";
                }
                else if (Input.GetAxis("LeftTiltControllerMac") > 0.1)
                {
                    RaiseEvent(LeanRightPressedEvent);
                    leanDir = "right";
                }
                else
                {
                    if (leanDir == "left")
                    {
                        RaiseEvent(LeanLeftReleasedEvent);
                        leanDir = "";
                    }
                    else if (leanDir == "right")
                    {
                        RaiseEvent(LeanRightReleasedEvent);
                        leanDir = "";
                    }
                }
            }
            else
            {
                if (Input.GetAxis("RightTiltControllerWin") > 0.1)
                {
                    RaiseEvent(LeanLeftPressedEvent);
                    leanDir = "left";
                }
                else if (Input.GetAxis("LeftTiltControllerWin") > 0.1)
                {
                    RaiseEvent(LeanRightPressedEvent);
                    leanDir = "right";
                }
                else
                {
                    if (leanDir == "left")
                    {
                        RaiseEvent(LeanLeftReleasedEvent);
                        leanDir = "";
                    }
                    else if (leanDir == "right")
                    {
                        RaiseEvent(LeanRightReleasedEvent);
                        leanDir = "";
                    }
                }
            }
        }
    }

    // Returns the player's movement on the horizontal axis in 3D and zero in 2D
    public float GetSideMovement()
    {
        if (GameStateManager.instance != null)
        {
            if (GameStateManager.instance.currentPerspective == PerspectiveType.p3D)
                return Input.GetAxis("Horizontal");
            else
                return 0f;
        }
        return 0f;
    }

    public float GetMenuMovement()
    {
        string axis = Mathf.Abs(Input.GetAxis("Vertical")) >= Mathf.Abs(Input.GetAxis("Horizontal")) ? "Vertical" : "Horizontal";
        string dpadAxis = Mathf.Abs(Input.GetAxis("Menu")) >= Mathf.Abs(Input.GetAxis("MenuSlider")) ? "Menu" : "MenuSlider";
        float stickInput = Input.GetAxis(axis);
        float dpadInput = Input.GetAxis(dpadAxis);
        bool usingDpad = false;
        if (Mathf.Abs(dpadInput) > Mathf.Abs(stickInput))
        {
            usingDpad = true;
            axis = dpadAxis == "Menu" ? "Vertical" : "Horizontal";
        }

        menuAxis = axis;
        if (usingDpad)
        {
            return dpadInput;
        }
        else
        {
            return stickInput;
        }
    }

    // Returns true if the jump button is currently pressed
    public bool JumpStatus()
    {
        return GetButtonAbsolute("Jump");
    }

    // Returns true if the interaction button is currently pressed
    public bool InteractStatus()
    {
        return GetButtonAbsolute("Interaction");
    }

    // Returns true if the grab button is currently pressed
    // NOTE: refers to crate grabbing
    public bool GrabStatus()
    {
        return GetButtonAbsolute("Grab");
    }

    public void SetFailFlag()
    {
        flipFailed = true;
    }

    #endregion Public Interface

    #region Event Raising Functions

    private void RaiseEvent(System.Action gameEvent)
    {
        if (gameEvent != null)
            gameEvent();
    }

    public void ContinuePressed()
    {
        continuePressed = true;
    }

    public void callShiftPressed()
    {
        if (GameStateManager.instance != null)
        {
            if (!GameStateManager.instance.changingPerspective())
                RaiseEvent(ShiftPressedEvent);
        }
    }

    #endregion Event Raising Functions
}

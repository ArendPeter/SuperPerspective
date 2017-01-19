using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

    public Rigidbody2D rb;
    public GameObject leftGroundCheck, rightGroundCheck;
    public bool grounded = true;
    RaycastHit2D leftHit;
    RaycastHit2D rightHit;
    Animator anim;
    public bool isActive = true;
    SpriteRenderer playerSprite;
    Color initCol;
    bool fadeOut;
    int facingForward = 1;
    public Fadeout2D fade;

    public Orb2D orb;

    public Transform middle, orbMount;

    private RaycastHit2D hit;
    public LayerMask mask;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        InputManager.instance.JumpPressedEvent += Jump;
        anim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        initCol = playerSprite.color;

        InputManager.instance.InteractPressedEvent += Interact;
    }

    void Interact() {
        hit = Physics2D.Raycast(middle.position, new Vector2(facingForward, 0), 0.35f, mask);
        if (hit.collider != null)
        {
            hit.collider.gameObject.GetComponent<Interactible2D>().Activate(this);
        }
    }

    void DetectMovement()
    {
        float temp = InputManager.instance.GetForwardMovement();
        if (temp > 0)
        {
            Move(new Vector2(temp * 3, rb.velocity.y));
            anim.SetBool("Walking", true);
            transform.localScale = new Vector3(1, 1, 1);
            facingForward = 1;
        }
        else if (temp < 0)
        {
            Move(new Vector2(temp * 3, rb.velocity.y));
            anim.SetBool("Walking", true);
            transform.localScale = new Vector3(-1, 1, 1);
            facingForward = -1;
        }
        else
        {
            Move(new Vector2(0, rb.velocity.y));
            anim.SetBool("Walking", false);
        }
    }

    public void Jump() {
        if (grounded)
        {
            rb.velocity = (new Vector3(rb.velocity.x, 6f));
        }
    }

    void Move(Vector3 movement)
    {
        rb.velocity = movement;
    }

    // Update is called once per frame
    void Update () {
        if (isActive)
        {
            leftHit = Physics2D.Raycast(leftGroundCheck.transform.position, new Vector2(0, -1f), 0.3f);
            rightHit = Physics2D.Raycast(rightGroundCheck.transform.position, new Vector2(0, -1f), 0.3f);
            grounded = ((leftHit.collider != null && leftHit.collider.name != "HoodieGirl2D") || (rightHit.collider != null && rightHit.collider.name != "HoodieGirl2D"));

            if (grounded)
            {
                anim.SetBool("Jump", false);
            }
            else
            {
                anim.SetBool("Jump", true);
            }
        }
    }

    public void StartTeleport()
    {
        anim.SetTrigger("Warp");
        fadeOut = true;
        isActive = false;
        rb.velocity = (new Vector3(0,0));
        fade.FadeIn();
    }

    public void EndTeleport(Vector2 newPos)
    {
        isActive = true;
        fadeOut = false;
        anim.SetTrigger("Idle");
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            DetectMovement();
        }
        if (fadeOut && playerSprite.color.a > 0)
        {
            playerSprite.color = new Color(initCol.r, initCol.b, initCol.g, playerSprite.color.a - 0.05f);
        }
        else if (!fadeOut)
        {
            playerSprite.color = initCol;
        }
    }
}

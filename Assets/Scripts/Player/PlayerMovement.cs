using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    string horizontalAxis = "Horizontal";
    string verticalAxis = "Vertical";
    float hMovement = 0f;
    float vMovement = 0f;
    float playerMass = 1;
    Vector2 gravityOffset;

    // Player movement
    public bool facingLeft;
    bool isRunning;
    bool onGround;
    bool jumpAttempt;
    bool canDoubleJump;
    bool hasDoubleJumped;
    float jumpMultiplier = 100f;
    float walkMultiplier = 1f;
    
    // Player abilities
    GameManager gm;
    Rigidbody2D rb;
    CapsuleCollider2D playerCollider;
    public PlayerCombat playerCombatScript; 
    [SerializeField] Animator animator;

    [SerializeField]
    float walkSpeed = 5f, runSpeed = 10f, jumpForce = 10f, doubleJumpForce = 15f;

    [SerializeField]
    private LayerMask levelLayerMask;    

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerCollider = gameObject.GetComponent<CapsuleCollider2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = playerMass;
    }

    void Awake()
    {
        facingLeft = true;
        isRunning = false;
        jumpAttempt = false;
        canDoubleJump = true;
        hasDoubleJumped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
            return;
        
        // Get input from user
        hMovement = Input.GetAxis(horizontalAxis);
        if (Input.GetKeyDown(gm.runKey))
        { // press run key
            isRunning = true;
        }
        if (Input.GetKeyUp(gm.runKey))
        { // let go of run key
            isRunning = false;
        }

        // Apply run speed if the character is running
        if (isRunning)
        {
            hMovement = hMovement * runSpeed;
        }
        else
        {
            hMovement = hMovement * walkSpeed;
        }
        animator.SetFloat("Speed", Mathf.Abs(hMovement));


        // Flip character to look the way they are moving
        if (hMovement > 0 && facingLeft)
        {
            Flip();
        }
        else if (hMovement < 0 && !facingLeft)
        {
            Flip();
        }

        // Attempt to jump
        if (Input.GetKeyDown(gm.jumpKey))
        {
            jumpAttempt = true;
        }        
    }

    void FixedUpdate()
    {
        if (!PauseMenu.isPaused)
        {
            /* Physics movements go here! */

        // Reset the velocity to zero
        MoveCharacter();
        onGround = isGrounded();
        animator.SetBool("isJumping", !onGround);
        if (onGround && jumpAttempt)
        {
            Jump();
            jumpAttempt = false;
            canDoubleJump = true;
            hasDoubleJumped = false;
        }
        else if (jumpAttempt && canDoubleJump && !hasDoubleJumped)
        {
            DoubleJump();
            hasDoubleJumped = true;
            canDoubleJump = false;
            jumpAttempt = false;
        }
        }

    }


    void MoveCharacter()
    {
        // Only apply horizontal movement
        rb.velocity = new Vector2(hMovement, rb.velocity.y);
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingLeft = !facingLeft;
    }
    void Jump() => rb.AddForce(new Vector2(rb.velocity.x, jumpForce*jumpMultiplier));

    void DoubleJump() 
    {
        // Reset y velocity to avoid force multiplication
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(rb.velocity.x, doubleJumpForce*jumpMultiplier));
    }


    // Returns a boolean value whether the player is on the ground or not (for jumping)
    private bool isGrounded()
    {
        float playerColliderOffset = 0.3f;
        RaycastHit2D raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + playerColliderOffset, levelLayerMask);
        return raycastHit.collider != null;
    }

}




// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;

// public class PlayerMovement : MonoBehaviour
// {
//     string horizontalAxis = "Horizontal";
//     string verticalAxis = "Vertical";
//     float hMovement = 0f;
//     float vMovement = 0f;
//     float playerMass = 1;
//     bool onGround;

//     PlayerState state;

//     // Player movement
//     bool facingLeft;
    
//     // Player abilities
//     GameManager gm;
//     Rigidbody2D rb;
//     CapsuleCollider2D playerCollider;
//     public PlayerCombat playerCombatScript; 
//     [SerializeField] Animator animator;

//     [SerializeField]
//     float walkSpeed = 5f, runSpeed = 10f, jumpForce = 50f;

//     [SerializeField]
//     private LayerMask levelLayerMask;    

//     // Start is called before the first frame update
//     void Start()
//     {
//         gm = GameObject.FindObjectOfType<GameManager>();
//         rb = gameObject.GetComponent<Rigidbody2D>();
//         playerCollider = gameObject.GetComponent<CapsuleCollider2D>();
//         rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//         rb.mass = playerMass;
//     }

//     void Awake()
//     {
//         facingLeft = true;
//     }

//     void Update()
//     {
//         state = GetState();
//         Debug.Log("State="+state.ToString());

//         animator.SetFloat("Speed", Mathf.Abs(hMovement)); // TODO??????????????????

//         // Flip character to look the way they are moving
//         FaceDirection();

//     }

//     void FixedUpdate()
//     {
//         /* Physics movements go here! */
//         // rb.velocity = new Vector2(hMovement, rb.velocity.y); // Continue moving character TODO ????

//         switch (state)
//         {
//             case PlayerState.Walk:
//                 Walk();
//                 break;
//             case PlayerState.Run:
//                 Run();
//                 break;
//             case PlayerState.Jump:
//                 Jump();
//                 break;
//             case PlayerState.Idle:
//                 // Do nothing
//                 break;
//         }
        
//         animator.SetBool("isJumping", !isGrounded());
        
//     }


//     PlayerState GetState()
//     {
//         onGround = isGrounded();
//         if (Input.GetKeyDown(gm.jumpKey) && onGround) 
//         {
//             return PlayerState.Jump;
//         }

//         hMovement = Input.GetAxis(horizontalAxis);
//         if (hMovement != 0)
//         {
//             if (Input.GetKey(gm.runKey))
//             {
//                 Debug.Log("LOLLLL");   
//                 return PlayerState.Run;
//             } 
//             else 
//             {
//                 return PlayerState.Walk;
//             }
//         }
        
//         else return PlayerState.Idle;
//     }

//     void Walk() => rb.velocity = new Vector2(hMovement*walkSpeed, rb.velocity.y);

//     void Run() =>  rb.velocity = new Vector2(hMovement*runSpeed, rb.velocity.y);

//     void Jump() => rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
    
//     void Flip()
//     {
//         Vector3 currentScale = gameObject.transform.localScale;
//         currentScale.x *= -1;
//         gameObject.transform.localScale = currentScale;

//         facingLeft = !facingLeft;
//     }

//     void FaceDirection()
//     {
//         if (hMovement > 0 && facingLeft)
//         {
//             Flip();
//         }
//         else if (hMovement < 0 && !facingLeft)
//         {
//             Flip();
//         }
//     }

//     // Returns a boolean value whether the player is on the ground or not (for jumping)
//     private bool isGrounded()
//     {
//         float playerColliderOffset = 0.3f;
//         RaycastHit2D raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + playerColliderOffset, levelLayerMask);
//         return raycastHit.collider != null;
//     }

//     private void OnDrawGizmos() {
//        float playerColliderOffset = 0.3f;
//        Gizmos.DrawLine(playerCollider.bounds.center, new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.extents.y + playerColliderOffset, playerCollider.bounds.center.z));
//     }

// }

// enum PlayerState
// {
//     Walk,
//     Run,
//     Jump,
//     DoubleJump,
//     Idle,
// }

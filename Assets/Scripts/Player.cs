using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float arrowSpeed = 10f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;

    private float horizontal;
    private bool jumpRequested;
    private bool isFacingRight = true;

    private void Awake()
    {
        // Auto-assign Rigidbody2D if not set
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // Auto-assign Animator
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get horizontal input
        horizontal = Input.GetAxisRaw("Horizontal");

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }

        // Flip sprite if needed
        HandleFlip();

        // Update animations
        HandleRunAnimation();
        //         var leftMouse = Input.GetMouseButtonDown(0);
        //         var rightMouse = Input.GetMouseButtonDown(1);
        // Shoot arrow
        if (Input.GetMouseButtonDown(0) && arrowPrefab != null && firePoint != null)
        {
            ShootArrow();
            animator.SetTrigger("Shoot");
        }
    }

    private void FixedUpdate()
    {
        // Check if grounded
        if (groundCheckPoint != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }

        HandleMovement();
        HandleJump();
        Debug.Log("Horizontal input: " + horizontal + " | Velocity: " + rb.velocity);
        Debug.Log("Grounded: " + isGrounded + " | Velocity: " + rb.velocity);
    }

    private void HandleMovement()
    {
        Debug.Log("We are moving");
        Vector2 velocity = rb.velocity;
        Debug.Log("We are moving_1");
        velocity.x = horizontal * speed;
        Debug.Log("We are moving_2");
        rb.velocity = velocity;
        Debug.Log("We are moving_4");

        //?
        // Vector2 currentVelocity = rb.velocity;
        // currentVelocity.x = horizontal * speed;

        //? Vertical movement is handled separately by gravity/jumping, 
        //? unless you are making a top-down game (then use vertical * speed).
        // rb.velocity = currentVelocity;

        //? using transform.position += move; is not recommended for physics-based movement,
        //? as it can cause tunneling issues (objects passing through each other) and ignores collisions

        // var move = new Vector3(horizontal * speed, 0f, 0f) * Time.deltaTime;
        // Debug.Log(move.ToString() + " This is the move vector");
        // transform.position += move;
        // Debug.Log(transform.position.ToString() + " This is the transform position");
    }

    private void HandleJump()
    {
        if (jumpRequested && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("isJumping", true);
            jumpRequested = false;
        }
    }

    // private void HandleRunAnimation()
    // {
    //     animator.SetFloat("isRunning", Mathf.Abs(horizontal));

    //     if (Mathf.Abs(horizontal) < 0.01f && isGrounded)
    //         animator.SetFloat("isRunning", 0f);
    // }
    private void HandleRunAnimation()
    {
        float absHorizontal = Mathf.Abs(horizontal);
        animator.SetFloat("isRunning", absHorizontal);

        // Stop running animation if below threshold
        if (absHorizontal < 0.1f && isGrounded)
            animator.SetFloat("isRunning", 0f);
    }
    private void HandleFlip()
    {
        if (horizontal < 0f && isFacingRight)
        {
            Flip();
        }
        else if (horizontal > 0f && !isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void ShootArrow()
    {
        GameObject arrowInstance = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D arrowRb = arrowInstance.GetComponent<Rigidbody2D>();
        float direction = isFacingRight ? 1f : -1f;
        arrowRb.velocity = arrowInstance.transform.right * arrowSpeed * direction;
        Destroy(arrowInstance, 5f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground") && col.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
            Debug.Log("Collode with ground");
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground"))
        {

            isGrounded = false;
            Debug.Log("Exit ground");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }

        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }
}

// using UnityEngine;

// public class Player : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     [SerializeField] private float speed = 5f;          // Horizontal movement speed
//     [SerializeField] private float jumpForce = 10f;     // Jump force
//     [SerializeField] private float arrowSpeed = 10f;    // Speed of arrows

//     [Header("Ground Check Settings")]
//     public Transform groundCheckPoint;                   // Empty object at feet to check ground
//     public float groundCheckRadius = 0.2f;              // Radius for ground check
//     public LayerMask groundLayer;                        // Layer considered as ground
//     private bool isGrounded;                             // True if player is on ground

//     [Header("References")]
//     public Rigidbody2D rb;                               // Player Rigidbody2D
//     private Animator animator;                            // Animator component
//     [SerializeField] private GameObject arrowPrefab;     // Arrow prefab to shoot
//     [SerializeField] private Transform firePoint;        // Where arrow spawns

//     private float horizontal;                             // Horizontal input
//     private bool jumpRequested;                           // Jump input flag
//     private bool isFacingRight = true;                    // Tracks player facing direction

//     void Start()
//     {
//         // Initialize components
//         if (rb == null) rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         isGrounded = true;
//         isFacingRight = true;
//     }

//     void Update()
//     {
//         // 1️⃣ Capture horizontal input
//         horizontal = Input.GetAxisRaw("Horizontal");
//         // vertical = Input.GetAxisRaw("Vertical");

//         // 2️⃣ Capture jump input
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             jumpRequested = true;
//         }

//         // 3️⃣ Flip sprite based on input
//         HandleFlip();

//         // 4️⃣ Run animation
//         HandleRunAnimation();

//         // 5️⃣ Shooting input
//         if (Input.GetMouseButtonDown(0)) // Left click
//         {
//             ShootArrow();
//             animator.SetTrigger("Shoot");
//         }
//     }

//     void FixedUpdate()
//     {
//         // 1️⃣ Handle horizontal movement
//         HandleMovement();

//         // 2️⃣ Handle jumping
//         HandleJump();

//         // 3️⃣ Update grounded status using OverlapCircle
//         isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
//     }

//     void HandleMovement()
//     {
//         // Move player horizontally using Rigidbody2D
//         Vector2 velocity = rb.velocity;
//         velocity.x = horizontal * speed;
//         rb.velocity = velocity;
//     }

//     void HandleJump()
//     {
//         if (jumpRequested && isGrounded)
//         {
//             Vector2 velocity = rb.velocity;
//             velocity.y = jumpForce;
//             rb.velocity = velocity;

//             animator.SetBool("isJumping", true);
//             jumpRequested = false;
//         }
//     }

//     void HandleRunAnimation()
//     {
//         // Set run speed in animator (assuming a float parameter "isRunning")
//         animator.SetFloat("isRunning", Mathf.Abs(horizontal));

//         // Stop running animation if not moving
//         if (Mathf.Abs(horizontal) < 0.01f && isGrounded)
//         {
//             animator.SetFloat("isRunning", 0f);
//         }
//     }

//     void HandleFlip()
//     {
//         // Flip sprite horizontally based on movement direction
//         if (horizontal < 0f && isFacingRight)
//         {
//             isFacingRight = false;
//             Vector3 scale = transform.localScale;
//             scale.x *= -1;
//             transform.localScale = scale;
//         }
//         else if (horizontal > 0f && !isFacingRight)
//         {
//             isFacingRight = true;
//             Vector3 scale = transform.localScale;
//             scale.x *= -1;
//             transform.localScale = scale;
//         }
//     }

//     void ShootArrow()
//     {
//         // Instantiate arrow prefab at fire point
//         GameObject arrowInstance = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

//         // Get Rigidbody2D from the arrow instance
//         Rigidbody2D arrowRb = arrowInstance.GetComponent<Rigidbody2D>();

//         // Set velocity in the direction the player is facing
//         float direction = isFacingRight ? 1f : -1f;
//         arrowRb.velocity = arrowInstance.transform.right * arrowSpeed * direction;

//         // Destroy arrow after 5 seconds
//         Destroy(arrowInstance, 5f);
//     }

//     private void OnCollisionEnter2D(Collision2D col)
//     {
//         // Check if player landed on ground
//         if (col.collider.CompareTag("Ground") && col.contacts[0].normal.y > 0.5f)
//         {
//             isGrounded = true;
//             animator.SetBool("isJumping", false);
//         }
//     }

//     private void OnCollisionExit2D(Collision2D col)
//     {
//         // Player left ground
//         if (col.collider.CompareTag("Ground"))
//         {
//             isGrounded = false;
//         }
//     }

//     private void OnDrawGizmosSelected()
//     {
//         // Draw ground check radius
//         if (groundCheckPoint != null)
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
//         }

//         // Draw fire point (optional)
//         if (firePoint != null)
//         {
//             Gizmos.color = Color.yellow;
//             Gizmos.DrawWireSphere(firePoint.position, 0.1f);
//         }
//     }
// }

// using UnityEngine;

// public class Player : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     [SerializeField] private float speed = 5f;
//     [SerializeField] private float jumpHeight = 100f;
//     private float arrowSpeed = 5f;
//     [Header("References")]
//     public Rigidbody2D rb;

//     private float horizontal;
//     private float vertical;
//     private bool jumpRequested;
//     private bool isGrounded;
//     private bool isFacingRight;
//     // public Transform player;
//     public Transform groundCheckPoint;
//     public float groundCheckRadius = .2f;
//     public LayerMask groundLayer;
//     private Animator animator;
//     [SerializeField] private GameObject arrowPrefab;
//     [SerializeField] private Transform firePoint;

//     //? Start is called before the first frame update
//     void Start()
//     {
//         isGrounded = true; //? Assume the player starts on the ground
//         isFacingRight = true;
//         //? Get the Rigidbody component attached to the GameObject
//         // player = GetComponent<Transform>();
//         // rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         animator = this.gameObject.GetComponent<Animator>(); //.SetBool("isRunning", false);
//     }
//     // Update: Called once per frame, regardless of frame rate. 

//     void Update()
//     {
//         //? 1. CAPTURE INPUT IN UPDATE
//         //? We poll inputs every frame to ensure we never miss a fast key press.
//         horizontal = Input.GetAxisRaw("Horizontal");
//         vertical = Input.GetAxisRaw("Vertical");

//         //? Use a boolean flag for the jump so FixedUpdate can "consume" it later.
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             jumpRequested = true;
//         }
//         Collider2D collider = OnLand();
//         Debug.Log(collider + " This is the collider from OnLand");
//         if (collider != null)
//         {
//             isGrounded = true;
//         }
//         else
//         {
//             isGrounded = false;
//         }
//         FlipUser();
//         PlayRunAmination();
//         var leftMouse = Input.GetMouseButtonDown(0);
//         var rightMouse = Input.GetMouseButtonDown(1);
//         if (leftMouse)
//         {
//             animator.SetTrigger("Shoot");
//             Debug.Log("Left mouse button clicked");
//         }
//         else if (rightMouse)
//         {
//             Debug.Log("Right mouse button clicked");
//         }
//         {
//             Debug.Log("Left mouse button not clicked");
//         }
//     }
//     /*
//     ? FixedUpdate: Called at fixed time intervals (default 0.02 seconds, or 50 times per second),
//     ? independent of frame rate. Use it for physics-related code, such as applying forces, 
//     ? moving rigidbodies, or collision detection, to ensure consistent behavior across different hardware.
//     */
//     private void FixedUpdate()
//     {
//         //? 2. APPLY PHYSICS IN FIXEDUPDATE
//         //? Physics calculations run at a fixed interval (default 0.02s).
//         HandleMovement();
//         HandleJump();
//     }

//     void PlayRunAmination()
//     {
//         if (Mathf.Abs(horizontal) > 0f && isGrounded)
//         {
//             // animator.SetBool("isRunning", true);
//             animator.SetFloat("isRunning", Mathf.Abs(horizontal));

//         }
//         else if (horizontal < 0.1f)
//         {
//             animator.SetFloat("isRunning", 0f);
//             // animator.SetBool("isRunning", false);
//         }
//     }
//     public void ShootArrow()
//     {
//         // Instantiate a new arrow prefab at firePoint
//         Debug.Log("Attempting to shoot arrow..." +
//                   " firePoint: " + firePoint.position +
//                   " firePoint.rotation: " + firePoint.rotation);

//         GameObject arrowInstance = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

//         //? VERY IMPORTANT:
//         //? We must get the Rigidbody2D from the arrow instance,
//         //? NOT use the player's rb.
//         Rigidbody2D arrowRb = arrowInstance.GetComponent<Rigidbody2D>();

//         //? Make the arrow move in the direction the player is facing.
//         //? transform.right uses the arrow's local X axis.
//         arrowRb.velocity = arrowInstance.transform.right * arrowSpeed;

//         // Destroy arrow after 5 seconds
//         //? Optional: destroy arrow after 5 seconds to avoid clutter.
//         Destroy(arrowInstance, 5f);
//     }
//     void HandleMovement()
//     {
//         //? Instead of modifying transform.position (which ignores collisions),
//         //? we set the Rigidbody's velocity for responsive platformer movement.
//         // Vector2 currentVelocity = rb.velocity;
//         // currentVelocity.x = horizontal * speed;

//         //? Vertical movement is handled separately by gravity/jumping, 
//         //? unless you are making a top-down game (then use vertical * speed).
//         // rb.velocity = currentVelocity;

//         //? using transform.position += move; is not recommended for physics-based movement,
//         //? as it can cause tunneling issues (objects passing through each other) and ignores collisions

//         var move = new Vector3(horizontal * speed, 0f, 0f) * Time.deltaTime;
//         // Debug.Log(move.ToString() + " This is the move vector");
//         transform.position += move;
//         // Debug.Log(transform.position.ToString() + " This is the transform position");
//     }

//     void HandleJump()
//     {
//         if (jumpRequested && isGrounded)
//         {
//             /*
//             APPROACH A: Direct Velocity Change (Instant)
//             Instantly overrides the Y velocity. Best for "snappy" arcade controls 
//             where the player must reach the same height every time.
//             */
//             Vector2 vector = rb.velocity;
//             // Debug.Log($"This is the vector {vector}");
//             vector.y = jumpHeight;
//             rb.velocity = vector;
//             animator.SetBool("isJumping", true);
//             /*
//             APPROACH B: AddForce with Impulse (Physics-based)
//             Newton's Law (F = ma) in action. It applies a sudden "kick" to the object.
//             The resulting height will vary if the object is already moving up or down.
//             Calculation: rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
//             */
//             // rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);

//             isGrounded = false; //? Assume the player is now in the air after jumping
//             jumpRequested = false;
//         }
//     }

//     void OnCollisionEnter2D(Collision2D col)
//     {
//         Debug.Log(col.collider.tag + " in OnCollisionEnter2D");
//         Debug.Log(col.contacts[0].normal + " This is the contact normal");
//         Debug.Log(col.gameObject.tag + " This is the collided object's tag");
//         Debug.Log(col.gameObject.tag == "Ground" + " This is the collided object's tag (gameObject)");
//         // only count collisions where the contact normal is “upwards”
//         if (col.collider.CompareTag("Ground") && col.contacts[0].normal.y > 0.5f)
//         {
//             isGrounded = true;
//             animator.SetBool("isJumping", false);

//         }
//     }

//     void OnCollisionExit2D(Collision2D col)
//     {
//         Debug.Log(col.collider.tag + " in OnCollisionExit2D");
//         // ✅ Fixed tag case
//         if (col.collider.CompareTag("Ground"))
//             isGrounded = false;
//     }
//     // manual ground check using OverlapCircle (alternative to collision events)
//     Collider2D OnLand()
//     {
//         Debug.Log("Checking for ground... in OnLand");
//         // Physics2D.OverlapCircle(Vector2.zero, 0.1f, LayerMask.GetMask("Ground"), 10, 0);
//         var a = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
//         return a;
//     }
//     private void OnDrawGizmosSelected()
//     {
//         if (groundCheckPoint == null)
//             return;

//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
//         Gizmos.color = Color.green;
//         Collider2D collider = OnLand();
//         if (collider != null)
//         {
//             Gizmos.DrawWireSphere(collider.transform.position, 0.1f);
//         }
//     }
//     private void FlipUser()
//     {
//         if (horizontal < 0f && isFacingRight)
//         {
//             transform.eulerAngles = new Vector3(0f, -180f, 0f);
//             isFacingRight = false;

//         }
//         else if (horizontal > 0f && !isFacingRight)
//         {

//             transform.eulerAngles = new Vector3(0f, 0f, 0f);
//             isFacingRight = true;
//         }

//     }
// }
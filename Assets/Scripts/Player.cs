using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 100f;
    private float arrowSpeed = 5f;
    [Header("References")]
    public Rigidbody2D rb;

    private float horizontal;
    private float vertical;
    private bool jumpRequested;
    private bool isGrounded;
    private bool isFacingRight;
    // public Transform player;
    public Transform groundCheckPoint;
    public float groundCheckRadius = .2f;
    public LayerMask groundLayer;
    private Animator animator;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;

    //? Start is called before the first frame update
    void Start()
    {
        isGrounded = true; //? Assume the player starts on the ground
        isFacingRight = true;
        //? Get the Rigidbody component attached to the GameObject
        // player = GetComponent<Transform>();
        // rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator = this.gameObject.GetComponent<Animator>(); //.SetBool("isRunning", false);
    }
    // Update: Called once per frame, regardless of frame rate. 

    void Update()
    {
        //? 1. CAPTURE INPUT IN UPDATE
        //? We poll inputs every frame to ensure we never miss a fast key press.
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //? Use a boolean flag for the jump so FixedUpdate can "consume" it later.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
        Collider2D collider = OnLand();
        Debug.Log(collider + " This is the collider from OnLand");
        if (collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        FlipUser();
        PlayRunAmination();
        var leftMouse = Input.GetMouseButtonDown(0);
        var rightMouse = Input.GetMouseButtonDown(1);
        if (leftMouse)
        {
            animator.SetTrigger("Shoot");
            Debug.Log("Left mouse button clicked");
        }
        else if (rightMouse)
        {
            Debug.Log("Right mouse button clicked");
        }
        {
            Debug.Log("Left mouse button not clicked");
        }
    }
    /*
    ? FixedUpdate: Called at fixed time intervals (default 0.02 seconds, or 50 times per second),
    ? independent of frame rate. Use it for physics-related code, such as applying forces, 
    ? moving rigidbodies, or collision detection, to ensure consistent behavior across different hardware.
    */
    private void FixedUpdate()
    {
        //? 2. APPLY PHYSICS IN FIXEDUPDATE
        //? Physics calculations run at a fixed interval (default 0.02s).
        HandleMovement();
        HandleJump();
    }

    void PlayRunAmination()
    {
        if (Mathf.Abs(horizontal) > 0f && isGrounded)
        {
            // animator.SetBool("isRunning", true);
            animator.SetFloat("isRunning", Mathf.Abs(horizontal));

        }
        else if (horizontal < 0.1f)
        {
            animator.SetFloat("isRunning", 0f);
            // animator.SetBool("isRunning", false);
        }
    }
    public void ShootArrow()
    {
        // Instantiate a new arrow prefab at firePoint
        Debug.Log("Attempting to shoot arrow..." +
                  " firePoint: " + firePoint.position +
                  " firePoint.rotation: " + firePoint.rotation);

        GameObject arrowInstance = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        //? VERY IMPORTANT:
        //? We must get the Rigidbody2D from the arrow instance,
        //? NOT use the player's rb.
        Rigidbody2D arrowRb = arrowInstance.GetComponent<Rigidbody2D>();

        //? Make the arrow move in the direction the player is facing.
        //? transform.right uses the arrow's local X axis.
        arrowRb.velocity = arrowInstance.transform.right * arrowSpeed;

        // Destroy arrow after 5 seconds
        //? Optional: destroy arrow after 5 seconds to avoid clutter.
        Destroy(arrowInstance, 5f);
    }
    void HandleMovement()
    {
        //? Instead of modifying transform.position (which ignores collisions),
        //? we set the Rigidbody's velocity for responsive platformer movement.
        // Vector2 currentVelocity = rb.velocity;
        // currentVelocity.x = horizontal * speed;

        //? Vertical movement is handled separately by gravity/jumping, 
        //? unless you are making a top-down game (then use vertical * speed).
        // rb.velocity = currentVelocity;

        //? using transform.position += move; is not recommended for physics-based movement,
        //? as it can cause tunneling issues (objects passing through each other) and ignores collisions

        var move = new Vector3(horizontal * speed, 0f, 0f) * Time.deltaTime;
        // Debug.Log(move.ToString() + " This is the move vector");
        transform.position += move;
        // Debug.Log(transform.position.ToString() + " This is the transform position");
    }

    void HandleJump()
    {
        if (jumpRequested && isGrounded)
        {
            /*
            APPROACH A: Direct Velocity Change (Instant)
            Instantly overrides the Y velocity. Best for "snappy" arcade controls 
            where the player must reach the same height every time.
            */
            Vector2 vector = rb.velocity;
            // Debug.Log($"This is the vector {vector}");
            vector.y = jumpHeight;
            rb.velocity = vector;
            animator.SetBool("isJumping", true);
            /*
            APPROACH B: AddForce with Impulse (Physics-based)
            Newton's Law (F = ma) in action. It applies a sudden "kick" to the object.
            The resulting height will vary if the object is already moving up or down.
            Calculation: rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            */
            // rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);

            isGrounded = false; //? Assume the player is now in the air after jumping
            jumpRequested = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(col.collider.tag + " in OnCollisionEnter2D");
        Debug.Log(col.contacts[0].normal + " This is the contact normal");
        Debug.Log(col.gameObject.tag + " This is the collided object's tag");
        Debug.Log(col.gameObject.tag == "Ground" + " This is the collided object's tag (gameObject)");
        // only count collisions where the contact normal is “upwards”
        if (col.collider.CompareTag("Ground") && col.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);

        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log(col.collider.tag + " in OnCollisionExit2D");
        // ✅ Fixed tag case
        if (col.collider.CompareTag("Ground"))
            isGrounded = false;
    }
    // manual ground check using OverlapCircle (alternative to collision events)
    Collider2D OnLand()
    {
        Debug.Log("Checking for ground... in OnLand");
        // Physics2D.OverlapCircle(Vector2.zero, 0.1f, LayerMask.GetMask("Ground"), 10, 0);
        var a = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        return a;
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        Gizmos.color = Color.green;
        Collider2D collider = OnLand();
        if (collider != null)
        {
            Gizmos.DrawWireSphere(collider.transform.position, 0.1f);
        }
    }
    private void FlipUser()
    {
        if (horizontal < 0f && isFacingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            isFacingRight = false;

        }
        else if (horizontal > 0f && !isFacingRight)
        {

            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            isFacingRight = true;
        }

    }
}
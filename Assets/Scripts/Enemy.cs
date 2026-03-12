using UnityEngine;

public class Enemy : MonoBehaviour
{
    private readonly float walkSpeed = 0.5f;

    public Transform groundCheckPoint;
    public float distance = 0.3f;
    public LayerMask groundLayer;

    private bool isFacingLeft;
    public float attackRangeRadius = 6f;
    public LayerMask targetLayers;


    void Start()
    {
        isFacingLeft = true;
    }

    void Update()
    {
        // Determine movement direction
        // If facing left → move -1
        // If facing right → move +1
        float direction = isFacingLeft ? -1 : 1;
        Collider2D collInfo = Physics2D.OverlapCircle(transform.position, attackRangeRadius, targetLayers);
        if (collInfo)
        {
            Debug.Log("we are close to player");
        }
        else
        {

            // Move the enemy horizontally
            // Time.deltaTime makes movement frame-rate independent
            transform.Translate(direction * Time.deltaTime * walkSpeed * Vector2.right);

            // transform.Translate(Vector2.right * direction * walkSpeed * Time.deltaTime);
            // transform.position += new Vector3(walkSpeed * Time.deltaTime, 0f, 0f); 
            // transform.Translate(new Vector3(walkSpeed * Time.deltaTime, 0f, 0f), Space.Self); transform.Translate(Time.deltaTime * walkSpeed * Vector2.left, Space.Self); // RaycastHit hit; // RaycastHit2D ray = Physics.Raycast(groundCheckPoint.position, Vector3.down, out hit, distance, groundLayer);

            // Shoot a ray downward from the groundCheckPoint
            // This checks if there is ground ahead

            // RaycastHit hit; 
            // RaycastHit2D ray = Physics.Raycast(groundCheckPoint.position, Vector3.down, out hit, distance, groundLayer);
            // RaycastHit2D hitInfo = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, distance, groundLayer); 
            // var hitInfo = Physics.Raycast(groundCheckPoint.position, Vector2.down, out hit, distance, groundLayer); 
            // if (hitInfo)
            // if (Physics.Raycast(groundCheckPoint.position, groundCheckPoint.forward, out hit, 10f))
            RaycastHit2D hitInfo = Physics2D.Raycast(
                groundCheckPoint.position,  // Starting position of the ray
                Vector2.down,               // Direction of the ray
                distance,                   // Length of the ray
                groundLayer                 // Only detect objects in this layer
            );

            // If the ray DOES NOT hit ground
            if (!hitInfo)
            {
                // Turn the enemy around
                Flip();
            }
        }
    }

    void Flip()
    {
        // Reverse the facing direction
        isFacingLeft = !isFacingLeft;

        // Flip the sprite by reversing its X scale
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        // transform.Rotate(0f, -180f, 0f); 
        // transform.eulerAngles = new Vector3(0f, -180f, 0f);
        // transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        // Prevent errors if the checkpoint isn't assigned
        if (groundCheckPoint == null) return;

        // Draw the ray in the editor so you can see where the ground check happens
        Gizmos.color = Color.red;

        // Draw a ray downward from the checkpoint
        Gizmos.DrawRay(groundCheckPoint.position, Vector2.down * distance);
    }
}
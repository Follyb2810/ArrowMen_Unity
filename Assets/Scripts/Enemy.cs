using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 0.9f;                 // Enemy movement speed
    private bool isFacingLeft = true;             // Tracks current facing direction

    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;            // Point to cast ray downward to check for edges
    public float distance = 0.3f;                 // Length of ground check ray
    public LayerMask groundLayer;                 // Layer considered as ground

    [Header("Player Detection Settings")]
    public float attackRangeRadius = 6f;          // Radius to detect player
    public LayerMask targetLayers;                // Layers considered as targets

    private Animator enemyAnimator;

    public Transform playerTranform;
    public float chaseSpeed = 2f;
    private float retrieveDistance = 3f;

    void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
    }


    void Start()
    {
        // Enemy starts facing left
        isFacingLeft = true;
    }

    void Update()
    {
        // 1️⃣ Detect player in attack range
        Collider2D collInfo = Physics2D.OverlapCircle(transform.position, attackRangeRadius, targetLayers);
        if (collInfo)
        {
            if (playerTranform.position.x > transform.position.x && isFacingLeft)
            {
                Flip();


                isFacingLeft = false;
            }
            else
            {
                Flip();
                isFacingLeft = true;
            }
            // Player detected within range
            Debug.Log("Player nearby: " + collInfo.name);
            Debug.Log("Player position: collInfo" + collInfo.transform.position.ToString());
            Debug.Log("Player real postion: playerTranform" + playerTranform.transform.position.ToString());
            Vector2 targetPos = new Vector2(playerTranform.position.x, transform.position.y);
            if (Vector2.Distance(transform.position, targetPos) >= retrieveDistance)
            {

                transform.position = Vector2.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);
                enemyAnimator.SetBool("isAttack", false);
            }
            else
            {
                enemyAnimator.SetBool("isAttack", true);

            }
            // transform.position = Vector2.MoveTowards(transform.position, playerTranform.transform.position, chaseSpeed * Time.deltaTime);
        }
        else
        {
            // 2️⃣ Patrol movement
            Patrol();
        }
    }

    void Patrol()
    {
        // Determine movement direction
        float direction = isFacingLeft ? -1 : 1;

        // Move horizontally
        transform.Translate(direction * Time.deltaTime * walkSpeed * Vector2.right);

        // 3️⃣ Ground detection using raycast
        RaycastHit2D hitInfo = Physics2D.Raycast(
            groundCheckPoint.position,  // Ray start
            Vector2.down,               // Ray direction
            distance,                   // Ray length
            groundLayer                 // Only hit ground layer
        );

        // If no ground detected ahead → flip direction
        if (!hitInfo)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Reverse facing direction
        isFacingLeft = !isFacingLeft;

        // Flip sprite horizontally by inverting localScale.x
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // transform.Rotate(0f, -180f, 0f); 
        // transform.eulerAngles = new Vector3(0f, -180f, 0f);
        // transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the ground check ray in the Scene view
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(groundCheckPoint.position, Vector2.down * distance);
        }

        // Draw player detection radius in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRangeRadius);
    }


}

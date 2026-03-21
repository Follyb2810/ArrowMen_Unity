using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private float arrowSpeed = 5f;
    private float destroyTime = 5f;
    private float horizontal;
    private float vertical;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Give the arrow velocity in the direction it’s facing
        rb.velocity = transform.right * arrowSpeed;
        // transform.right points along the arrow’s local X axis
        // This works even if player is flipped horizontally
        Destroy(gameObject, destroyTime);
    }
    // public void SpawnArrow()
    // {
    //     var arrow = Instantiate(gameObject, transform.position, transform.rotation);
    // }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {

        // Vector3 direction = new Vector3(1, 2, 0);
        // transform.Translate(Vector3.right * Time.deltaTime);
        // transform.Translate(Vector3.right * 5f * Time.deltaTime);
        // transform.Translate(Vector3.right * 5f * Time.deltaTime, Space.Self);
        // transform.Translate(Vector3.right * arrowSpeed * Time.deltaTime, Space.World);
        // Using Translate works, but since your arrow has a Rigidbody2D, better practice is to use physics:
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

    }
    void MoveArrow()
    {
        if (horizontal < 0f)
        {
            rb.velocity = transform.right * arrowSpeed;

        }
        else if (horizontal > 0f)
        {
            rb.velocity = transform.right * arrowSpeed;
        }
    }
}

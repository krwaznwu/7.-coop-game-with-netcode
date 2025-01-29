using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    //   private void Update()
    //    {
    //        if (!IsOwner) return;
    //
    //        Vector3 moveDir = new Vector3(0, 0, 0);
    //
    //        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f; 
    //        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f; 
    //        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f; 
    //        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;
    //        
    //        float moveSpeed = 3f;
    //        
    //        transform.position += moveDir * moveSpeed * Time.deltaTime;
    //
    //    }

    public float moveSpeed = 20f; // Movement speed
    public float jumpForce = 8f; // Force applied when jumping
    public LayerMask groundLayer; // Layer used to identify the ground
    public Transform groundCheck; // Position to check for ground

    private Rigidbody rb; // Reference to Rigidbody component
    private bool isGrounded; // Is the player on the ground?

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDir = Vector3.zero;

        // Movement input
        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        // Normalize movement direction and apply speed
        moveDir = moveDir.normalized * moveSpeed;

        // Update position (horizontal movement)
        Vector3 newPosition = transform.position + moveDir * Time.deltaTime;
        rb.MovePosition(new Vector3(newPosition.x, rb.position.y, newPosition.z));

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Ground check using a small sphere at the groundCheck position
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        // Visualize ground check in editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }


}

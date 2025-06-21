using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxMoveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    public float playerHealth = 100f; 

    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 currentVelocity;
    private bool facingRight = true;
    private bool isAttacking = false;
    private bool isGatheringResources = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Don't process movement input while attacking
        if (isAttacking || isGatheringResources)
        {
            movement = Vector3.zero;
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        movement = new Vector3(moveX, 0f, moveZ).normalized;

        // Handle flipping based on horizontal movement
        if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveX < 0 && facingRight)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (movement.magnitude > 0)
        {
            Vector3 targetVelocity = movement * maxMoveSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // Public methods for combat system to control movement
    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
    }
    public void SetGatheringResources(bool gathering)
    {
        isGatheringResources = gathering;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
    public bool IsGatheringResources()
    {
        return isGatheringResources;
    }
}
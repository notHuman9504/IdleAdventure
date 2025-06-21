using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 70f;
    private Transform player;
    private bool facingRight = false;
    private Rigidbody rb;
    private Animator animator;
    public int attackDamage = 10;

    public Transform attackBoxCenter;
    public Vector3 attackBoxSize = new Vector3(2f, 2f, 2f);
    public Vector3 attackBoxRotation;
    public LayerMask playerLayer;
    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
        }
        // Ensure enemy starts facing left
        if (facingRight)
        {
            Flip();
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange && distanceToPlayer > 1f)
            {
                // Flip to face player
                if (player.position.x > transform.position.x && !facingRight)
                {
                    Flip();
                }
                else if (player.position.x < transform.position.x && facingRight)
                {
                    Flip();
                }
                // Move towards player (X/Z only)
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0;
                rb.velocity = direction * moveSpeed;

            }
            else
            {
                rb.velocity = Vector3.zero;
            }
            if (!isAttacking)
                {
                    Collider[] hitPlayers = Physics.OverlapBox(
                        attackBoxCenter.position,
                        attackBoxSize * 0.5f,
                        Quaternion.Euler(attackBoxRotation),
                        playerLayer);
                    if (hitPlayers.Length > 0)
                    {
                        TriggerAttackAnimation();
                        isAttacking = true;
                    }
                }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // Call this from an animation event to deal damage at the perfect frame
    public void DealDamageToPlayer()
    {
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.playerHealth -= attackDamage;
                if (playerMovement.playerHealth < 0) playerMovement.playerHealth = 0;
                Debug.Log($"Player took {attackDamage} damage. Health: {playerMovement.playerHealth}");
            }
        }
    }

    // Call this from animation event when attack animation ends
    public void AttackFinished()
    {
        isAttacking = false;
    }

    // Visualize the attack box in the editor
    void OnDrawGizmosSelected()
    {
        if (attackBoxCenter != null)
        {
            Gizmos.color = Color.yellow;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(attackBoxCenter.position, Quaternion.Euler(attackBoxRotation), Vector3.one);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}

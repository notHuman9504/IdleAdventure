using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    [Header("Attack Settings")]
    public Transform attackPoint;  // Point for combat attacks
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;

    [Header("Chopping Settings")]
    public Transform choppingPoint;  // Separate point for chopping
    public float choppingRange = 1f;
    public LayerMask treeLayers;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        
        // If no separate chopping point is assigned, use attack point
        if (choppingPoint == null)
        {
            choppingPoint = attackPoint;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !playerMovement.IsAttacking() && !playerMovement.IsGatheringResources())
        {
            // Check for enemies using attack sphere
            Collider[] enemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
            if (enemies.Length > 0)
            {
                Attack();
                return;
            }

            // Check for trees using chopping sphere
            Collider[] trees = Physics.OverlapSphere(choppingPoint.position, choppingRange, treeLayers);
            if(trees.Length > 0)
            {
                Chopping();
                return;
            }
            Attack();
        }
    }

    void Attack()
    {
        playerMovement.SetAttacking(true);
        animator.SetTrigger("PlayerAttack");
    }

    void Chopping()
    {
        playerMovement.SetGatheringResources(true);
        animator.SetTrigger("PlayerChop");
    }

    public void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Enemy hit!");
            enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage, transform);
        }
    }

    public void Chop()
    {
        Collider[] hitTrees = Physics.OverlapSphere(choppingPoint.position, choppingRange, treeLayers);
        foreach (Collider tree in hitTrees)
        {
            Debug.Log("Tree chopped!");
            tree.GetComponent<Enemy>()?.TakeDamage(attackDamage);
        }
    }

    // Called by animation event when attack animation ends
    public void AttackFinished()
    {
        playerMovement.SetAttacking(false);
    }

    public void ResourceGatheringFinished()
    {
        playerMovement.SetGatheringResources(false);
    }

    void OnDrawGizmosSelected()
    {
        // Draw attack sphere in red
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        
        // Draw chopping sphere in green
        if (choppingPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(choppingPoint.position, choppingRange);
        }
    }
}
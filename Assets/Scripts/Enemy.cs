using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemDrop
{
    public string itemName;
    public int quantity;

    public ItemDrop(string name, int qty)
    {
        itemName = name;
        quantity = qty;
    }
}

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Item Drops")]
    public ItemDrop[] itemDrops = new ItemDrop[]
    {
        new ItemDrop("Wood", 5),
        new ItemDrop("Seed", 1)
    };

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    [Header("Knockback Settings")]
    public bool isActive = false;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.25f;
    private Rigidbody rb;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Vector3 knockbackVelocity;

    void Start()
    {
        currentHealth = maxHealth;
        originalPosition = transform.localPosition;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            knockbackTimer += Time.fixedDeltaTime;
            if (knockbackTimer >= knockbackDuration)
            {
                isKnockedBack = false;
                knockbackTimer = 0f;
                // Reset velocity to zero after knockback
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                }
            }
            else if (rb != null)
            {
                // Apply smooth knockback velocity
                float progress = knockbackTimer / knockbackDuration;
                float smoothFactor = Mathf.Lerp(1f, 0f, progress);
                rb.velocity = knockbackVelocity * smoothFactor;
            }
        }
    }

    public void TakeDamage(int damage, Transform player = null)
    {
        if (isActive && player != null && rb != null)
        {
            // Calculate direction from player to enemy
            Vector3 direction = (transform.position - player.position).normalized;
            // Keep knockback horizontal only
            direction.y = 0;
            
            // Set knockback state and velocity
            isKnockedBack = true;
            knockbackTimer = 0f;
            knockbackVelocity = direction * knockbackForce;
            
            // Initial impulse
            rb.velocity = knockbackVelocity;
        }
        else if (!isActive)
        {
            // Start shake effect
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeX(shakeDuration, shakeMagnitude));
        }

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator ShakeX(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPosition + new Vector3(offsetX, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        DropItems();

        Destroy(gameObject);
    }

    void DropItems()
    {
        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("No InventorySystem found! Make sure you have an InventorySystem in the scene.");
            return;
        }

        // Add each item drop to the player's inventory
        foreach (ItemDrop drop in itemDrops)
        {
            InventorySystem.Instance.AddItem(drop.itemName, drop.quantity);
        }

        Debug.Log($"{gameObject.name} dropped items to player's bag!");
    }

    // Public method to get current health (for UI or other systems)
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Public method to get max health
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    // Method to heal (if needed)
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"{gameObject.name} healed for {healAmount}. Health: {currentHealth}/{maxHealth}");
    }
}
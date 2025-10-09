using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int quantity;

    public InventoryItem(string name, int qty)
    {
        itemName = name;
        quantity = qty;
    }
}

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();

    void Awake()
    {
        // Singleton pattern - only one inventory system
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add items to inventory
    public void AddItem(string itemName, int quantity)
    {
        // Check if item already exists in inventory
        InventoryItem existingItem = inventory.Find(item => item.itemName == itemName);

        if (existingItem != null)
        {
            // Item exists, add to quantity
            existingItem.quantity += quantity;
        }
        else
        {
            // New item, add to inventory
            inventory.Add(new InventoryItem(itemName, quantity));
        }

        Debug.Log($"Added {quantity} {itemName}(s) to inventory. Total: {GetItemQuantity(itemName)}");

        // Update UI if you have one
        UpdateInventoryUI();
    }

    // Remove items from inventory
    public bool RemoveItem(string itemName, int quantity)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemName == itemName);

        if (existingItem != null && existingItem.quantity >= quantity)
        {
            existingItem.quantity -= quantity;

            // Remove item if quantity reaches 0
            if (existingItem.quantity <= 0)
            {
                inventory.Remove(existingItem);
            }

            Debug.Log($"Removed {quantity} {itemName}(s) from inventory");
            UpdateInventoryUI();
            return true;
        }

        Debug.Log($"Not enough {itemName} in inventory");
        return false;
    }

    // Get quantity of specific item
    public int GetItemQuantity(string itemName)
    {
        InventoryItem item = inventory.Find(i => i.itemName == itemName);
        return item != null ? item.quantity : 0;
    }

    // Get all items (for UI display)
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(inventory);
    }

    // Clear entire inventory
    public void ClearInventory()
    {
        inventory.Clear();
        UpdateInventoryUI();
        Debug.Log("Inventory cleared");
    }

    // Print inventory contents to console
    public void PrintInventory()
    {
        Debug.Log("=== INVENTORY ===");
        if (inventory.Count == 0)
        {
            Debug.Log("Inventory is empty");
            return;
        }

        foreach (InventoryItem item in inventory)
        {
            Debug.Log($"{item.itemName}: {item.quantity}");
        }
        Debug.Log("================");
    }

    // Call UI updates
    private void UpdateInventoryUI()
    {
        InventoryUI ui = FindObjectOfType<InventoryUI>();
        if (ui != null)
        {
            ui.UpdateInventoryDisplay();
        }
    }
}
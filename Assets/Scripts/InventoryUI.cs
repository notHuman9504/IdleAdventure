using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public Text inventoryText;
    
    [Header("Settings")]
    public bool showEmptyMessage = true;
    public string emptyInventoryMessage = "Inventory is empty";

    void Start()
    {
        if (inventoryText == null)
        {
            Debug.LogError("Inventory Text is not assigned! Please assign it in the Inspector.");
        }
        
        UpdateInventoryDisplay();
    }

    void Update()
    {
        // Update the display every frame to keep it in sync
        UpdateInventoryDisplay();
    }

    public void UpdateInventoryDisplay()
    {
        if (inventoryText == null || InventorySystem.Instance == null)
            return;

        List<InventoryItem> items = InventorySystem.Instance.GetAllItems();

        if (items.Count == 0)
        {
            inventoryText.text = showEmptyMessage ? emptyInventoryMessage : "";
            return;
        }

        // Build the inventory text
        string displayText = "=== INVENTORY ===\n";
        
        foreach (InventoryItem item in items)
        {
            displayText += $"{item.itemName}: {item.quantity}\n";
        }

        inventoryText.text = displayText;
    }
}
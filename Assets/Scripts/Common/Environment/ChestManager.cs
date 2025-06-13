using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ChestManager handles generating random items from the database and letting the player pick one.
/// </summary>
public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance { get; private set; }

    [Header("Number of items to show when opening chest")]
    public int rewardCount = 3;

    private List<ItemBase> currentChestItems;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// When called, generates a new set of random items and returns them for display.
    /// </summary>
    public List<ItemBase> OpenChest()
    {
        currentChestItems = ItemDatabase.Instance.GetRandomItemsAllTypes(rewardCount);
        return currentChestItems;
    }

    /// <summary>
    /// Player picks an item by index. Adds to inventory, clears others.
    /// Returns true if success.
    /// </summary>
    public bool PickItem(int index)
    {
        if (currentChestItems == null || index < 0 || index >= currentChestItems.Count)
            return false;

        ItemBase picked = currentChestItems[index];

        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogError("[ChestManager] PlayerInventory not found.");
            return false;
        }

        bool added = inventory.AddItem(picked);
        if (!added)
        {
            Debug.Log("[ChestManager] Inventory full. Cannot add item from chest.");
            return false;
        }

        // Optional: If you want, you can notify UI or play animation here

        // Clear the chest, making sure only one item can be taken
        currentChestItems = null;
        return true;
    }

    /// <summary>
    /// Optional helper if you need to display current chest contents (e.g. for UI refresh).
    /// </summary>
    public List<ItemBase> GetCurrentChestItems()
    {
        return currentChestItems;
    }
}

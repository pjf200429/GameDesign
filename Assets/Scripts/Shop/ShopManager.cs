using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ShopManager is responsible for:
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("How many items to show in a single shop refresh (cannot exceed slot count)")]
    public int itemCount = 3;

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
    /// Returns a list of itemCount random ItemBase objects from all categories.
    /// Delegates to ItemDatabase.GetRandomItemsAllTypes().
    /// </summary>
    public List<ItemBase> GetRandomShopItems()
    {
        return ItemDatabase.Instance.GetRandomItemsAllTypes(itemCount);
    }

    /// <summary>
    /// Attempts to purchase the given item.
    /// Returns true if purchase succeeds (player has enough currency, item added to inventory).
    /// Returns false if purchase fails (insufficient currency, inventory full, etc.).
    /// </summary>
    public bool TryBuyItem(ItemBase item)
    {
        if (item == null)
            return false;

        // 1. Get PlayerAttributes instance and attempt to spend currency
        PlayerAttributes playerAttr = FindObjectOfType<PlayerAttributes>();
        if (playerAttr == null)
        {
            Debug.LogError("[ShopManager] PlayerAttributes instance not found. Purchase failed.");
            return false;
        }

        int price = item.Price;
        bool hasEnough = playerAttr.SpendCurrency(price);
        if (!hasEnough)
        {
            Debug.Log("[ShopManager] Purchase failed: insufficient currency.");
            return false;
        }

        // 2. After spending currency, add the item to the player's inventory
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogError("[ShopManager] PlayerInventory instance not found. Purchase failed.");
            // Return currency if inventory is missing
            playerAttr.AddCurrency(price);
            return false;
        }

        bool added = inventory.AddItem(item);
        if (!added)
        {
            // Inventory is full or adding failed, refund currency
            playerAttr.AddCurrency(price);
            Debug.Log("[ShopManager] Inventory full. Purchase failed and currency refunded.");
            return false;
        }

        // 3. Purchase succeeded
        Debug.Log($"[ShopManager] Successfully purchased {item.DisplayName} for {price} currency.");
        return true;
    }
}

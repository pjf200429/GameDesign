using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// ShopUI handles all UI-related behavior for the in-game shop:
/// </summary>
public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("¡ª¡ª Shop Slot Configuration (drag in the Inspector) ¡ª¡ª")]
    [Tooltip("List of ShopSlotUI components. Size must match the number of visible slots.")]
    public List<ShopSlotUI> slotUIs; // e.g. 3 slots

    [Header("¡ª¡ª Bottom Buttons (drag in the Inspector) ¡ª¡ª")]
    public Button detailButton;   // ¡°Detail¡± button
    public Button buyButton;      // ¡°Buy¡± button
    public Button exitButton;     // ¡°Exit¡± button

    [Header("¡ª¡ª Detail Panel (drag in the Inspector) ¡ª¡ª")]
    public GameObject detailPanel;         // Root GameObject of the detail popup
    public TMP_Text detailNameText;        // Text component for item name in detail popup
    public TMP_Text detailDescriptionText; // Text component for item description
    public Image detailIconImage;          // Image component for item icon
    public Button detailCloseButton;       // Button to close the detail popup

    // Tracks which slot index is currently selected (-1 means ¡°none selected¡±)
    private int currentSelectedIndex = -1;

    // Keeps the list of ItemBase objects currently shown in the slots
    private List<ItemBase> currentShopItems = new List<ItemBase>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("[ShopUI] Multiple instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // Hide the detail panel at startup
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    private void Start()
    {
        // Bind button callbacks
        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(OnDetailButtonClicked);
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        if (detailCloseButton != null && detailPanel != null)
        {
            detailCloseButton.onClick.RemoveAllListeners();
            detailCloseButton.onClick.AddListener(() => detailPanel.SetActive(false));
        }

        // Populate slots for the first time
        RefreshShopUI();
    }

    /// <summary>
    /// Called by ShopSlotUI when a slot is clicked.
    /// Sets highlight on the clicked slot; clears highlights on others.
    /// </summary>
    public void OnSelectSlot(ShopSlotUI selectedSlot)
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (slotUIs[i] == selectedSlot)
            {
                currentSelectedIndex = i;
                slotUIs[i].SetHighlight(true);
            }
            else
            {
                slotUIs[i].SetHighlight(false);
            }
        }
    }

    /// <summary>
    /// Called when ¡°Detail¡± button is clicked.
    /// If a valid slot is selected, shows item details in the detail panel.
    /// </summary>
    private void OnDetailButtonClicked()
    {
        //if (currentSelectedIndex < 0 || currentSelectedIndex >= currentShopItems.Count)
        //    return;

        //ItemBase item = currentShopItems[currentSelectedIndex];
        //if (item == null || detailPanel == null)
        //    return;

        //// Fill detail popup fields (assuming ItemBase has Description property)
        //detailNameText.text = item.DisplayName;
        //detailDescriptionText.text = item.Description;
        //detailIconImage.sprite = item.Icon;
        //detailPanel.SetActive(true);
    }

    /// <summary>
    /// Called when ¡°Buy¡± button is clicked.
    /// Attempts to buy one unit of the selected item:
    ///  - If purchase succeeds, decrements slot quantity and removes slot if quantity reaches zero.
    ///  - If purchase fails, logs a warning.
    /// </summary>
    private void OnBuyButtonClicked()
    {
        if (currentSelectedIndex < 0 || currentSelectedIndex >= currentShopItems.Count)
            return;

        ItemBase item = currentShopItems[currentSelectedIndex];
        if (item == null)
            return;

        bool purchaseSucceeded = ShopManager.Instance.TryBuyItem(item);
        if (purchaseSucceeded)
        {
            // Decrement quantity in the slot
            ShopSlotUI slot = slotUIs[currentSelectedIndex];
            bool stillHasQuantity = slot.DecrementQuantity();

            if (!stillHasQuantity)
            {
                // If quantity is now 0, remove the item from both UI and data list
                currentShopItems[currentSelectedIndex] = null;
                slot.SetItem(null);
                slot.gameObject.SetActive(false);
            }

            // Reset selection index
            currentSelectedIndex = -1;
        }
        else
        {
            Debug.LogWarning("[ShopUI] Purchase failed¡ªinsufficient funds or inventory full.");
        }
    }

    /// <summary>
    /// Called when ¡°Exit¡± button is clicked.
    /// Closes the shop by calling UIManager.CloseShop().
    /// </summary>
    private void OnExitButtonClicked()
    {
        UIManager.Instance.CloseShop();
    }

    /// <summary>
    /// Fetches a new random list of items from ShopManager and updates all slotUIs.
    /// Clears highlights and hides unused slots.
    /// </summary>
    private void RefreshShopUI()
    {
        // Reset selection index
        currentSelectedIndex = -1;

        // Clear highlights on all slots
        foreach (var slot in slotUIs)
            slot.SetHighlight(false);

        // 1. Ask ShopManager for a fresh batch of random items
        currentShopItems = ShopManager.Instance.GetRandomShopItems();

        // 2. Fill each slot with an item (or hide the slot if there's no item for that index)
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < currentShopItems.Count)
            {
                slotUIs[i].gameObject.SetActive(true);
                slotUIs[i].SetItem(currentShopItems[i]);
            }
            else
            {
                slotUIs[i].SetItem(null);
                slotUIs[i].gameObject.SetActive(false);
            }
        }
    }
}

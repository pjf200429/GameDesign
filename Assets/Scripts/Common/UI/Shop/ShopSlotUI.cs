using UnityEngine;
using UnityEngine.UI;
using TMPro; 

/// <summary>
/// Handles UI logic for a single shop slot:
/// </summary>
public class ShopSlotUI : MonoBehaviour
{
    [Header("UI Child References (assign in the Inspector)")]
    public Image iconImage;           // Displays the item icon
    public TMP_Text nameText;         // Displays the item name
    public TMP_Text priceText;        // Displays the item price
    public GameObject highlightFrame; // Highlight overlay (assign to ¡°ShopSlot_X/Highlight¡±)
    public TMP_Text quantityText;     // Displays purchase quantity (assign to ¡°ShopSlot_X/QuantityText¡±)

    [HideInInspector]
    public ItemBase currentItem;      // The ItemBase currently assigned to this slot

    // How many units can be purchased for this slot
    public int CurrentQuantity { get; private set; }

    private Button _button;           // The Button component used to detect clicks

    private void Awake()
    {
        // Hide the highlight overlay at startup
        if (highlightFrame != null)
            highlightFrame.SetActive(false);

        // Hide the quantity text at startup
        if (quantityText != null)
            quantityText.gameObject.SetActive(false);

        // Try to find a Button component on this GameObject
        _button = GetComponent<Button>();
        // If no Button on the root, search among children
        if (_button == null)
            _button = GetComponentInChildren<Button>();

        if (_button != null)
            _button.onClick.AddListener(OnSlotClicked);
        else
            Debug.LogWarning($"[ShopSlotUI] Awake: {gameObject.name} has no Button component on root or children!");
    }

    /// <summary>
    /// Assigns an ItemBase to this slot and updates icon, name, price, and purchase quantity.
    /// </summary>
    public void SetItem(ItemBase item)
    {
        currentItem = item;

        if (item != null)
        {
            // Display icon if available
            if (item.Icon != null)
            {
                iconImage.sprite = item.Icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            // Display name and price
            nameText.text = item.DisplayName;
            priceText.text = "Cost: " + item.Price.ToString();

            // Determine purchase quantity: 1¨C4 for consumables, otherwise 1
            if (item is ConsumableItem)
                CurrentQuantity = Random.Range(1, 5); // Random between 1 and 4
            else
                CurrentQuantity = 1;

            // Show quantity text
            if (quantityText != null)
            {
                quantityText.text = $"x{CurrentQuantity}";
                quantityText.gameObject.SetActive(true);
            }
        }
        else
        {
            // If item is null, clear icon/name/price and hide quantity
            iconImage.sprite = null;
            iconImage.enabled = false;
            nameText.text = "";
            priceText.text = "";

            if (quantityText != null)
                quantityText.gameObject.SetActive(false);

            CurrentQuantity = 0;
        }

        // Always hide highlight when setting a new item
        if (highlightFrame != null)
            highlightFrame.SetActive(false);
    }

    /// <summary>
    /// Reduces CurrentQuantity by one and updates the UI text.
    /// Returns true if after decrement the quantity is still > 0; 
    /// returns false if quantity reaches 0.
    /// </summary>
    public bool DecrementQuantity()
    {
        if (CurrentQuantity <= 0)
            return false;

        CurrentQuantity--;
        if (quantityText != null)
        {
            if (CurrentQuantity > 0)
                quantityText.text = $"x{CurrentQuantity}";
            else
                quantityText.gameObject.SetActive(false);
        }

        return (CurrentQuantity > 0);
    }

    /// <summary>
    /// Called when this slot is clicked. Notifies ShopUI about the selection.
    /// </summary>
    private void OnSlotClicked()
    {
        Debug.Log($"[ShopSlotUI] Clicked {gameObject.name}");
        if (ShopUI.Instance != null)
            ShopUI.Instance.OnSelectSlot(this);
        else
            Debug.LogWarning("[ShopSlotUI] OnSlotClicked: ShopUI.Instance is null!");
    }

    /// <summary>
    /// Toggles the highlight overlay on or off.
    /// </summary>
    public void SetHighlight(bool flag)
    {
        if (highlightFrame != null)
            highlightFrame.SetActive(flag);
    }
}

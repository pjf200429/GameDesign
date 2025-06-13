using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SlotUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header(" Image ")]
    public Image iconImage;     

    [Header("TMP_Text ")]
    public TMP_Text countText;   

    [HideInInspector]
    public int slotIndex;          

    [HideInInspector]
    public InventoryUI inventoryUI; 

    private CanvasGroup canvasGroup;

    private void Awake()
    {
      
        if (iconImage != null)
        {
            iconImage.enabled = false;
        }

        if (countText != null)
        {
            countText.enabled = false;
            countText.text = "";
        }

     
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
   
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

  
    public void SetIcon(Sprite icon)
    {
        if (iconImage == null)
            return;

        if (icon != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = icon;
        }
        else
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }

   
    public void SetCount(int qty)
    {
        if (countText == null)
            return;

        if (qty > 1)
        {
            countText.enabled = true;
            countText.text = qty.ToString();
        }
        else
        {
            countText.enabled = false;
            countText.text = "";
        }
    }

   

   
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"Slot clicked: index={slotIndex}");

        
            if (inventoryUI != null)
            {
                var items = inventoryUI.playerInventory.Items;
                if (slotIndex < items.Count && items[slotIndex] != null)
                {
                
                    inventoryUI.ShowOptionPanelAt(slotIndex, eventData.position);
                }
                else
                {
               
                    Debug.Log($"Slot {slotIndex} is empty; no panel shown.");
                }
            }
        }
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
     
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
     
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

       
    }

    
}

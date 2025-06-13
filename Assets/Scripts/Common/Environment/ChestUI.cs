using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChestUI : MonoBehaviour
{
    public List<Button> itemSlots; 
    public List<Image> itemIcons;  

    private Chest chest;
    private List<ItemBase> items;

 
    private int lastClickedIndex = -1;
    private float lastClickTime = 0f;
    private const float doubleClickDelay = 0.3f;


    public void Init(Chest ownerChest)
    {
        chest = ownerChest;
        items = ChestManager.Instance.OpenChest();
        Debug.Log(itemSlots.Count);
        for (int i = 0; i < itemSlots.Count; i++)
        {
            int idx = i; 
            if (i < items.Count)
            {
              
                itemIcons[i].sprite = items[i].Icon; 
              
                itemIcons[i].gameObject.SetActive(true);
                itemSlots[i].interactable = true;

    
                itemSlots[i].onClick.RemoveAllListeners();
                itemSlots[i].onClick.AddListener(() => OnSlotClicked(idx));
            }
            else
            {
                itemIcons[i].gameObject.SetActive(false);
                itemSlots[i].interactable = false;
            }
        }
    }

    //Double click to choose
    private void OnSlotClicked(int index)
    {
        float t = Time.time;
        if (lastClickedIndex == index && t - lastClickTime < doubleClickDelay)
        {
            
            if (ChestManager.Instance.PickItem(index))
            {
                chest.CloseChest(); 
                Destroy(gameObject); 
            }
        }
        else
        {
            lastClickedIndex = index;
            lastClickTime = t;
        }
    }
}

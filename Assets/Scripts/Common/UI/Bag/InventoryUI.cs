using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;


public class InventoryUI : MonoBehaviour
{
    [Header("SlotPrefab")]
    public GameObject slotPrefab;

    [Header("SlotGridArea")]
    public Transform slotParent;

    [Header("PlayerInventory")]
    public PlayerInventory playerInventory;

    [Header("Scene")]
    public string bootSceneName = "Boot";

    private bool boundAfterBoot = false;
    private List<SlotUI> slotUIs = new List<SlotUI>();
    private int slotCount = 15;

    [Header("SlotOptionPanel")]
    public GameObject slotOptionPanel;
    [Header("Button")]
    public Button btnDetails;
    public Button btnDrop;
    public Button btnUse;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
       
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            go.name = "Slot_" + i;
            var ui = go.GetComponent<SlotUI>();
            ui.slotIndex = i;
            ui.inventoryUI = this;
            slotUIs.Add(ui);
        }
        slotOptionPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        UnbindPlayerInventory();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

   
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!boundAfterBoot && scene.name != bootSceneName)
        {
            boundAfterBoot = true;
            BindPlayerInventory();
        }
    }

    
    private void BindPlayerInventory()
    {
        UnbindPlayerInventory();
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += RefreshUI;
                RefreshUI();
                return;
            }
        }
        Debug.LogError("[InventoryUI] 未找到 PlayerInventory 组件，确保玩家预制体设置了 Tag=Player 且挂载了 PlayerInventory");
    }

    private void UnbindPlayerInventory()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= RefreshUI;
    }

   
    public void RefreshUI()
    {
        if (playerInventory == null) return;
        var items = playerInventory.Items;
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                slotUIs[i].SetIcon(items[i].Icon);
                int qty = (items[i] is ConsumableItem c) ? c.Quantity : 1;
                slotUIs[i].SetCount(qty);
            }
            else
            {
                slotUIs[i].SetIcon(null);
                slotUIs[i].SetCount(0);
            }
        }
    }

    
    public void ShowOptionPanelAt(int slotIndex, Vector2 screenPos)
    {
        slotOptionPanel.SetActive(true);
        var panelRect = slotOptionPanel.GetComponent<RectTransform>();
        var parentRect = slotOptionPanel.transform.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, screenPos, null, out Vector2 localPoint);
        panelRect.anchoredPosition = localPoint + new Vector2(10f, -10f);

        btnDetails.onClick.RemoveAllListeners();
        btnDrop.onClick.RemoveAllListeners();
        btnUse.onClick.RemoveAllListeners();

        btnDetails.onClick.AddListener(() => OnClick_Details(slotIndex));
        btnDrop.onClick.AddListener(() => OnClick_Drop(slotIndex));
        btnUse.onClick.AddListener(() => OnClick_Use(slotIndex));
    }

    private void OnClick_Details(int slotIndex)
    {
        slotOptionPanel.SetActive(false);
        if (slotIndex < playerInventory.Items.Count)
        {
            var item = playerInventory.Items[slotIndex];
            if (item != null)
                Debug.Log($"详情：{item.DisplayName} (ID:{item.ItemID})");
        }
    }
    private void OnClick_Drop(int slotIndex)
    {
        slotOptionPanel.SetActive(false);
        if (slotIndex < playerInventory.Items.Count)
        {
            var item = playerInventory.Items[slotIndex];
            if (item != null)
                playerInventory.RemoveItem(item);
        }
    }
    private void OnClick_Use(int slotIndex)
    {
        slotOptionPanel.SetActive(false);
        if (slotIndex < playerInventory.Items.Count)
        {
            var item = playerInventory.Items[slotIndex];
            if (item != null)
                playerInventory.UseItem(item, playerInventory.gameObject);
        }
    }
}

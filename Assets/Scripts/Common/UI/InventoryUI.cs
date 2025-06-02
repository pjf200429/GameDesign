using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;  // 如果按钮是 TextMeshPro，需要引用该命名空间

/// <summary>
/// InventoryUI：
/// 1. 运行时动态实例化若干格子（SlotPrefab）到 SlotGridArea；
/// 2. 监听 PlayerInventory 的变化事件，自动刷新每个格子的 Icon/数量；
/// 3. 弹出 SlotOptionPanel（包含三个按钮：详情、丢弃、使用）；
/// 4. 初始时调用一次刷新，确保 UI 与背包保持同步。
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("格子预制体 (SlotPrefab)")]
    [Tooltip("将包含 SlotUI 脚本的预制体拖到此处")]
    public GameObject slotPrefab;

    [Header("格子父节点 (SlotGridArea)")]
    [Tooltip("UI 层级中，挂了 GridLayoutGroup 的空物体")]
    public Transform slotParent;

    [Header("玩家背包 (PlayerInventory)")]
    [Tooltip("场景中挂了 PlayerInventory 的玩家对象")]
    public PlayerInventory playerInventory;

    // 运行时实例化出来的所有格子对应的 SlotUI 组件
    private List<SlotUI> slotUIs = new List<SlotUI>();

    // 若背包容量固定，这里统一用 15 个格子。也可以根据 playerInventory.maxSlots 动态调整。
    private int slotCount = 15;

    [Header("操作面板 (SlotOptionPanel)")]
    [Tooltip("“详情/丢弃/使用”三个按钮所在的父面板")]
    public GameObject slotOptionPanel;

    [Header("操作面板按钮引用")]
    public Button btnDetails;  // Inspector 拖入“详情”按钮
    public Button btnDrop;     // Inspector 拖入“丢弃”按钮
    public Button btnUse;      // Inspector 拖入“使用”按钮

    private void Awake()
    {
        // 如果在 Inspector 里没拖 playerInventory，就自动查找
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
                Debug.LogError("[InventoryUI] 场景中找不到 PlayerInventory，请在 Inspector 里手动赋值！");
        }
    }

    private void Start()
    {
        // 校验：slotPrefab、slotParent、playerInventory、slotOptionPanel、各按钮，必须在 Inspector 里赋好
        bool error = false;
        if (slotPrefab == null)
        {
            Debug.LogError("[InventoryUI] 请在 Inspector 里把 slotPrefab 拖好！");
            error = true;
        }
        if (slotParent == null)
        {
            Debug.LogError("[InventoryUI] 请在 Inspector 里把 slotParent 拖好！");
            error = true;
        }
        if (playerInventory == null)
        {
            Debug.LogError("[InventoryUI] 请在 Inspector 里把 playerInventory 拖好！");
            error = true;
        }
        if (slotOptionPanel == null)
        {
            Debug.LogError("[InventoryUI] 请在 Inspector 里把 slotOptionPanel （操作面板）拖好！");
            error = true;
        }
        if (btnDetails == null || btnDrop == null || btnUse == null)
        {
            Debug.LogError("[InventoryUI] 请在 Inspector 里把 btnDetails/btnDrop/btnUse 三个按钮拖好！");
            error = true;
        }
        if (error) return;

        // 1. 动态实例化 slotCount 个格子
        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            go.name = "Slot_" + i;
            SlotUI ui = go.GetComponent<SlotUI>();
            if (ui == null)
            {
                Debug.LogError("[InventoryUI] SlotPrefab 预制体的根节点缺少 SlotUI 脚本！");
                Destroy(go);
                continue;
            }

            // 让 SlotUI 知道它自己的索引，以及能回调到 InventoryUI
            ui.slotIndex = i;
            ui.inventoryUI = this;

            slotUIs.Add(ui);
        }

        // 2. 隐藏操作面板，待点击格子时再显示
        slotOptionPanel.SetActive(false);

        // 3. 订阅背包变化事件：PlayerInventory.Add/Remove/Use 内部会触发 OnInventoryChanged
        playerInventory.OnInventoryChanged += RefreshUI;

        // 4. 首次手动刷新一次 UI，确保初始画面跟背包状态一致
        RefreshUI();
    }

    /// <summary>
    /// 刷新 UI，把 playerInventory.Items 按索引映射到对应的格子里，
    /// 超过 Items.Count 的格子则清空（Icon=null, Count=0）。
    /// </summary>
    public void RefreshUI()
    {
        IReadOnlyList<ItemBase> items = playerInventory.Items;

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                slotUIs[i].SetIcon(items[i].Icon);

                int qty = 1;
                if (items[i] is ConsumableItem consumable)
                {
                    qty = consumable.Quantity;
                }
                slotUIs[i].SetCount(qty);
            }
            else
            {
                slotUIs[i].SetIcon(null);
                slotUIs[i].SetCount(0);
            }
        }
    }

    private void OnDestroy()
    {
        // 取消订阅，避免场景切换报错
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= RefreshUI;
    }

    /// <summary>
    /// 在鼠标点击某个格子时，由 SlotUI 调用：
    /// 在鼠标屏幕坐标 screenPos 附近将操作面板显示出来，并给按钮注册回调。
    /// </summary>
    /// <param name="slotIndex">被点击的格子索引</param>
    /// <param name="screenPos">鼠标点击时的屏幕坐标</param>
    public void ShowOptionPanelAt(int slotIndex, Vector2 screenPos)
    {
        // 1. 激活面板
        slotOptionPanel.SetActive(true);

        // 2. 计算在父 Canvas 本地坐标系中的位置（假设 Canvas 为 Screen Space - Overlay 类型）
        RectTransform panelRect = slotOptionPanel.GetComponent<RectTransform>();
        RectTransform parentRect = slotOptionPanel.transform.parent as RectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPos,
            null,       // Screen Space - Overlay 时摄像机参数为 null
            out localPoint);

        // 3. 加一个小偏移，避免面板遮住鼠标指针
        panelRect.anchoredPosition = localPoint + new Vector2(10f, -10f);

        // 4. 先把旧的点击事件移除，防止重复注册
        btnDetails.onClick.RemoveAllListeners();
        btnDrop.onClick.RemoveAllListeners();
        btnUse.onClick.RemoveAllListeners();

        // 5. 注册新的回调，传入 slotIndex
        btnDetails.onClick.AddListener(() => OnClick_Details(slotIndex));
        btnDrop.onClick.AddListener(() => OnClick_Drop(slotIndex));
        btnUse.onClick.AddListener(() => OnClick_Use(slotIndex));
    }

    /// <summary>
    /// 点击“详情”按钮时的回调接口，slotIndex 表示是哪一个格子被点击
    /// </summary>
    private void OnClick_Details(int slotIndex)
    {
        // 隐藏面板
        slotOptionPanel.SetActive(false);

        // 获取对应的 ItemBase
        if (slotIndex < playerInventory.Items.Count)
        {
            ItemBase item = playerInventory.Items[slotIndex];
            if (item != null)
            {
                // TODO: 在这里实现 “查看该物品详细信息” 的逻辑
                Debug.Log($"[InventoryUI] 详情：{item.DisplayName} (ID:{item.ItemID})");
            }
        }
    }

    /// <summary>
    /// 点击“丢弃”按钮时的回调接口
    /// </summary>
    private void OnClick_Drop(int slotIndex)
    {
        // 隐藏面板
        slotOptionPanel.SetActive(false);

        if (slotIndex < playerInventory.Items.Count)
        {
            ItemBase item = playerInventory.Items[slotIndex];
            if (item != null)
            {
                // TODO: 在这里实现 “从背包里移除该物品” 的逻辑
                playerInventory.RemoveItem(item);
                Debug.Log($"[InventoryUI] 丢弃：{item.DisplayName} (ID:{item.ItemID})");
            }
        }
    }

    /// <summary>
    /// 点击“使用”按钮时的回调接口
    /// </summary>
    private void OnClick_Use(int slotIndex)
    {
        // 隐藏面板
        slotOptionPanel.SetActive(false);

        if (slotIndex < playerInventory.Items.Count)
        {
            ItemBase item = playerInventory.Items[slotIndex];
            if (item != null)
            {
                playerInventory.UseItem(item, playerInventory.gameObject);
                Debug.Log($"[InventoryUI] 使用：{item.DisplayName} (ID:{item.ItemID})");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 每个格子的 UI 逻辑，使用 TextMeshPro 来显示堆叠数量
/// 需要挂在 SlotPrefab 根节点上，并且预制体下存在两个子节点：
///   1. IconImage (UI→Image)  —— 用于显示物品图标
///   2. CountText (UI→TextMeshPro - Text) —— 用于显示数量
///
/// 同时实现了：
///   - 鼠标左键点击→如果本格子有物品则弹出操作面板，否则不响应
///   - 鼠标右键点击→（后续可自行扩展）
///   - 拖拽接口留空
/// </summary>
public class SlotUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("图标 Image (子节点)")]
    public Image iconImage;      // Inspector 拖入子节点 IconImage 上的 Image 组件

    [Header("数量 TMP_Text (子节点)")]
    public TMP_Text countText;   // Inspector 拖入子节点 CountText 上的 TMP_Text 组件

    [HideInInspector]
    public int slotIndex;           // 运行时由 InventoryUI 赋值：该格子的索引

    [HideInInspector]
    public InventoryUI inventoryUI; // 运行时由 InventoryUI 赋值：用于回调 ShowOptionPanelAt(...)

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // 初始隐藏图标和数量
        if (iconImage != null)
        {
            iconImage.enabled = false;
        }

        if (countText != null)
        {
            countText.enabled = false;
            countText.text = "";
        }

        // 缓存 CanvasGroup，以便拖拽时调整半透明和射线穿透
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // 如果没有挂 CanvasGroup，就自动添加一个，以便拖拽过程中控制 alpha / blocksRaycasts
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// 设置格子内要显示的图标。如果 icon 为 null，则隐藏；
    /// 否则启用并赋给 iconImage.sprite。
    /// </summary>
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

    /// <summary>
    /// 设置堆叠数量。如果 qty <= 1，则隐藏文本；否则显示并更新数值
    /// </summary>
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

    #region 鼠标和拖拽交互接口

    /// <summary>
    /// 当玩家点击该格子时：左键如果该格子有物品，则弹出操作面板；否则不做任何操作。
    /// 右键点击可自行扩展逻辑
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"Slot clicked: index={slotIndex}");

            // 先判断该槽位是否有物品，如果没有则直接返回，不弹面板
            if (inventoryUI != null)
            {
                var items = inventoryUI.playerInventory.Items;
                if (slotIndex < items.Count && items[slotIndex] != null)
                {
                    // 如果该格子有物品，则弹出操作面板
                    inventoryUI.ShowOptionPanelAt(slotIndex, eventData.position);
                }
                else
                {
                    // 该格子为空，什么也不做
                    Debug.Log($"Slot {slotIndex} is empty; no panel shown.");
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 右键点击：可自行扩展“右键直接丢弃”之类逻辑
            Debug.Log($"[SlotUI] 右键点击格子：索引 {slotIndex}");
            // TODO: 在这里处理右键操作（如直接丢弃/快速使用等）
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标悬停时可以显示 Tooltip 等信息，留空或自行扩展
        // Debug.Log($"[SlotUI] 鼠标悬停格子：{slotIndex}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 隐藏 Tooltip 等信息，留空或自行扩展
        // Debug.Log($"[SlotUI] 鼠标离开格子：{slotIndex}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 开始拖拽时，让格子半透明并且不拦截射线
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
        // TODO: 如果你要做拖拽浮动图标，可在这里把 iconImage 复制到一个悬浮 Image 上
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 拖拽进行中，可实现浮动图标跟随鼠标
        // 例如：
        // floatingIconRect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 拖拽结束，恢复格子不透明并重新拦截射线
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // TODO: 处理拖拽到另一个格子的交换逻辑，例如：
        // if (eventData.pointerEnter != null)
        // {
        //     SlotUI targetSlot = eventData.pointerEnter.GetComponent<SlotUI>();
        //     if (targetSlot != null && targetSlot != this)
        //     {
        //         SwapItemWith(targetSlot);
        //     }
        // }
    }

    #endregion
}

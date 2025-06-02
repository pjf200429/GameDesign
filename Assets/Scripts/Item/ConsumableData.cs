// ConsumableData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ConsumableData")]
public class ConsumableData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("可消耗物品的唯一 ID，用于在 ItemDatabase 中查找")]
    public string ConsumableID;
    [Tooltip("在 UI 中显示的名称")]
    public string DisplayName;
    [Tooltip("物品图标（Inventory/掉落等界面展示）")]
    public Sprite Icon;

    [Header("使用效果")]
    [Tooltip("直接回血数量（如果此类型可回血）")]
    public int HealAmount = 0;
    [Tooltip("要施加的 Buff 类型（枚举 BuffType，自行定义）")]
    public BuffType BuffType = BuffType.None;
    [Tooltip("Buff 持续时间，单位：秒。如果不加 Buff 可填 0")]
    public float BuffDuration = 0f;
    [Tooltip("Buff 强度数值，比如攻击提升百分比，或者减速值等")]
    public float BuffValue = 0f;

    [Header("可选：堆叠及价格信息")]
    [Tooltip("是否可堆叠（多个相同 ID）")]
    public bool IsStackable = false;
    [Tooltip("单件售价（若未来商店需求）")]
    public int Price = 0;
}

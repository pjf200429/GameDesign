using UnityEngine;

/// <summary>
/// 所有“可装备”数据（武器、头盔、护甲）都继承自这个基类
/// </summary>
public abstract class EquipmentData : ScriptableObject
{
    [Header("基础信息")]
    public string EquipmentID;    // 唯一标识，所有子类共用
    public string DisplayName;    // UI 显示名称
    public Sprite Icon;           // 物品图标
    [Tooltip("单件售价（若未来商店需求）")]
    public int Price = 0;
}

// ItemBase.cs
// 抽象基类：所有物品都继承自此类，定义通用接口
using UnityEngine;

/// <summary>
/// 物品类型枚举，用于区分不同类别的物品（武器、消耗品等）
/// </summary>
public enum ItemType
{
    Weapon,
    Consumable,
    // 以后可根据需要扩展：Armor、Material、QuestItem 等
}

public abstract class ItemBase
{
    // 唯一 ID，用于与数据库和 DropTable 对应
    public string ItemID { get; protected set; }

    // 显示用名称、图标等
    public string DisplayName { get; protected set; }
    public Sprite Icon { get; protected set; }

    // 每个子类必须重写，返回对应的 ItemType
    public abstract ItemType Type { get; }

    /// <summary>
    /// 使用物品的抽象方法，由子类实现
    /// target 通常是 GameObject（玩家、敌人等）
    /// </summary>
    public abstract void Use(GameObject target);
}

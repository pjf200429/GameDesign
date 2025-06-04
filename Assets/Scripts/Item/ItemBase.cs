// ItemBase.cs
using UnityEngine;

/// <summary>
/// Item 类型枚举，用于区分不同类别的物品（武器、消耗品、头盔、护甲等）
/// </summary>
public enum ItemType
{
    Weapon,
    Consumable,
    Helmet,
    Armor
    // 后面可根据需要扩展：Material、QuestItem 等
}

/// <summary>
/// 抽象基类：所有物品都继承自此类，定义通用接口
/// </summary>
public abstract class ItemBase
{
    // 唯一 ID，用于跟数据库、掉落表对应
    public string ItemID { get; protected set; }

    // UI 显示名称、图标
    public string DisplayName { get; protected set; }
    public Sprite Icon { get; protected set; }

    // 价格：只有本类或子类可以在构造时赋值，外部无法随意改
    public int Price { get; protected set; }

    // 每个子类必须重写，返回对应的 ItemType
    public abstract ItemType Type { get; }

    /// <summary>
    /// 构造函数：所有子类都需要在创建时传入 price
    /// </summary>
    protected ItemBase(string itemId, string displayName, Sprite icon, int price)
    {
        ItemID = itemId;
        DisplayName = displayName;
        Icon = icon;
        Price = price;
    }

    /// <summary>
    /// 使用物品的抽象方法，由子类实现
    /// target 通常是 GameObject（玩家、敌人等）
    /// </summary>
    public abstract void Use(GameObject target);
}

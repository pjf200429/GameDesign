using UnityEngine;

/// <summary>
/// 可消耗物品类：继承自 ItemBase，支持回血和挂 Buff 的功能
/// </summary>
public class ConsumableItem : ItemBase
{
    // 可消耗品使用后回复的生命值
    public int HealAmount { get; private set; }

    // 对应的 Buff 类型（如果不是 Buff 道具，可设为 BuffType.None）
    public BuffType BuffType { get; private set; }

    // Buff 持续时长（秒）
    public float BuffDuration { get; private set; }

    // Buff 数值（例如：DefenseUp 用 30，AttackUp 用 0.3f 等）
    public float BuffValue { get; private set; }

    // 记录当前该消耗品在背包里的堆叠数量
    public int Quantity { get; private set; }

    /// <summary>
    /// 构造一个新的 ConsumableItem 实例，初始数量为 1
    /// </summary>
    public ConsumableItem(
        string itemId,
        string displayName,
        Sprite icon,
        int healAmount,
        BuffType buffType,
        float buffDuration,
        float buffValue)
    {
        ItemID = itemId;
        DisplayName = displayName;
        Icon = icon;
        HealAmount = healAmount;
        BuffType = buffType;
        BuffDuration = buffDuration;
        BuffValue = buffValue;

        // 新创建时，默认堆叠数量为 1
        Quantity = 1;
    }

    public override ItemType Type => ItemType.Consumable;

    /// <summary>
    /// 使用物品时调用：先回血，然后挂 Buff（如果 BuffType != None），最后自动减少数量
    /// </summary>
    public override void Use(GameObject target)
    {
        if (target == null) return;

        // 1) 回血逻辑
        if (HealAmount > 0)
        {
            var health = target.GetComponent<PlayerHealthController>();
            if (health != null)
            {
                health.Heal(HealAmount);
            }
        }

        // 2) 挂 Buff 逻辑
        if (BuffType != BuffType.None && BuffDuration > 0f)
        {
            var buffManager = target.GetComponent<BuffManager>();
            if (buffManager != null)
            {
                // 默认不叠加，同类型 Buff 刷新持续时间
                buffManager.AddBuff(BuffType, BuffDuration, BuffValue, 1, false);
            }
            else
            {
                Debug.LogWarning($"[ConsumableItem] 目标 {target.name} 没有挂 BuffManager，无法添加 Buff {BuffType}");
            }
        }

      
    }

    /// <summary>
    /// 给当前实例增加数量（用于堆叠）
    /// </summary>
    public void AddQuantity(int amount)
    {
        if (amount <= 0) return;
        Quantity += amount;
        Debug.Log($"[ConsumableItem] 堆叠 {DisplayName}，新增数量 = {amount}，当前总数量 = {Quantity}");
    }

    /// <summary>
    /// 减少当前实例的数量。如果数量降至 0，外部（PlayerInventory）需移除该实例
    /// </summary>
    public void ReduceQuantity(int amount)
    {
        if (amount <= 0) return;
        Quantity = Mathf.Max(0, Quantity - amount);
        Debug.Log($"[ConsumableItem] 减少 {DisplayName} 数量 = {amount}，剩余数量 = {Quantity}");
    }
}

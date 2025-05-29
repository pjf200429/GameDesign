//// ConsumableItem.cs
//// 消耗型物品：例如药水、增益道具
//using UnityEngine;

//public class ConsumableItem : ItemBase
//{
//    // 可能的回复量、Buff 类型等来自 ItemData
//    public int HealAmount { get; private set; }
//    public BuffType BuffType { get; private set; }
//    public float BuffDuration { get; private set; }
//    public float BuffValue { get; private set; }

//    public ConsumableItem(
//        string itemId,
//        string displayName,
//        Sprite icon,
//        int healAmount,
//        BuffType buff,
//        float duration,
//        float value)
//    {
//        ItemID = itemId;
//        DisplayName = displayName;
//        Icon = icon;
//        HealAmount = healAmount;
//        BuffType = buff;
//        BuffDuration = duration;
//        BuffValue = value;
//    }

//    public override void Use(GameObject target)
//    {
//        // 使用时恢复生命
//        var health = target.GetComponent<PlayerHealthController>();
//        if (health != null && HealAmount > 0)
//        {
//            health.Heal(HealAmount);
//        }
//        // 使用时添加 Buff
//        if (BuffDuration > 0 && BuffType != BuffType.None)
//        {
//            BuffManager.Instance.AddBuff(BuffType, BuffDuration, BuffValue);
//        }
//    }
//}
